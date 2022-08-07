using System.Diagnostics;
using Kantaiko.Hosting.Managed.Exceptions;
using Kantaiko.Hosting.Managed.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

/// <inheritdoc />
public class ManagedHost : IManagedHost
{
    private readonly IManagedHostHandler? _managedHostHandler;
    private readonly IHostBuilderFactoryProvider _hostBuilderFactoryProvider;

    private TaskCompletionSource? _shutdownTaskCompletionSource;
    private TaskCompletionSource<IRuntimeHostState>? _restartTaskCompletionSource;

    private CancellationToken _shutdownCancellationToken = CancellationToken.None;

    private ManagedHostState _state = ManagedHostState.NotStarted;

    public ManagedHostState State
    {
        get => _state;
        private set
        {
            _state = value;
            StateChanged?.Invoke(this, _state);
        }
    }

    public event EventHandler<ManagedHostState>? StateChanged;

    public bool IsStarted => State is ManagedHostState.Started;

    private IHostBuilderFactory? _currentHostBuilderFactory;
    private IHost? _currentHost;

    private readonly object _stateLock = new();

    public IServiceProvider Services
    {
        get
        {
            lock (_stateLock)
            {
                CheckHostState(ManagedHostState.Started);
            }

            Debug.Assert(_currentHost is not null);
            return _currentHost.Services;
        }
    }

    public ManagedHost(IHostBuilderFactoryProvider? hostBuilderFactoryProvider = null,
        IManagedHostHandler? managedHostHandler = null)
    {
        hostBuilderFactoryProvider ??= new SingleHostBuilderFactoryProvider(new DefaultHostBuilderFactory());
        _hostBuilderFactoryProvider = hostBuilderFactoryProvider;

        _managedHostHandler = managedHostHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.NotStarted);
            State = ManagedHostState.Starting;
        }

        // Initialize task completion sources
        _shutdownTaskCompletionSource = new TaskCompletionSource();
        _restartTaskCompletionSource = new TaskCompletionSource<IRuntimeHostState>();

        try
        {
            // Get new host builder factory
            _currentHostBuilderFactory = _hostBuilderFactoryProvider.GetHostBuilderFactory();

            // Build new host
            _currentHost = BuildHost(_currentHostBuilderFactory);

            // Start current host
            await _currentHost.StartAsync(cancellationToken);

            // Register shutdown handler to perform host restart
            RegisterShutdownHandler();

            if (State is not ManagedHostState.Starting)
            {
                throw new InvalidOperationException("Invalid internal host state");
            }

            if (_managedHostHandler is not null)
            {
                await _managedHostHandler.HandleInitialHostStartAsync(_currentHost.Services, cancellationToken);
            }

            State = ManagedHostState.Started;
        }
        catch (Exception exception)
        {
            State = ManagedHostState.NotStarted;

            _shutdownTaskCompletionSource?.SetException(exception);
            _shutdownTaskCompletionSource = null;

            _restartTaskCompletionSource?.SetException(exception);
            _restartTaskCompletionSource = null;

            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started);
            State = ManagedHostState.Stopping;
        }

        // Set shutdown cancellation token to allow force shutdown
        _shutdownCancellationToken = cancellationToken;

        Debug.Assert(_currentHost is not null);

        // Request host stop
        var runtimeHostManager = _currentHost.Services.GetRequiredService<IRuntimeHostManager>();
        runtimeHostManager.Stop();

        var completionSource = _shutdownTaskCompletionSource;
        return completionSource?.Task ?? Task.CompletedTask;
    }

    public Task<IRuntimeHostState> RestartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started);
            State = ManagedHostState.Restarting;
        }

        Debug.Assert(_currentHost is not null);

        // Request host restart
        var runtimeHostManager = _currentHost.Services.GetRequiredService<IRuntimeHostManager>();
        runtimeHostManager.Restart();

        return WaitForRestartAsync(cancellationToken);
    }

    public async Task WaitForShutdownAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started, ManagedHostState.Stopping);
        }

        Debug.Assert(_shutdownTaskCompletionSource is not null);

        await await Task.WhenAny(_shutdownTaskCompletionSource.Task,
            TaskHelper.CreateTaskFromCancellationToken(cancellationToken));
    }

    public async Task<IRuntimeHostState> WaitForRestartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started, ManagedHostState.Restarting);
        }

        Debug.Assert(_restartTaskCompletionSource is not null);

        return await await Task.WhenAny(_restartTaskCompletionSource.Task,
            TaskHelper.CreateTaskFromCancellationToken<IRuntimeHostState>(cancellationToken));
    }

    public void Dispose()
    {
        _currentHost?.Dispose();
    }

    private void UpdateHostState()
    {
        Debug.Assert(_currentHost is not null);

        var hostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

        State = hostState.RestartRequested ? ManagedHostState.Restarting : ManagedHostState.Stopping;
    }

    private async Task HandleHostShutdown(CancellationToken cancellationToken)
    {
        Debug.Assert(_currentHost is not null);
        Debug.Assert(_restartTaskCompletionSource is not null);

        // Get old host state
        var oldHostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

        // Dispose old host
        _currentHost.Dispose();
        _currentHost = null;

        if (!oldHostState.RestartRequested)
        {
            // Restart was not requested, so host is going to shutdown

            Debug.Assert(_shutdownTaskCompletionSource is not null);

            lock (_stateLock)
            {
                // Change host state
                State = ManagedHostState.NotStarted;

                // Notify shutdown listeners
                _shutdownTaskCompletionSource.SetResult();
                _shutdownTaskCompletionSource = null;

                // Cancel restart listeners
                _restartTaskCompletionSource.SetCanceled(CancellationToken.None);
                _restartTaskCompletionSource = null;
            }

            return;
        }

        try
        {
            // Try to restart host
            await PerformRestart(oldHostState, cancellationToken);
        }
        catch (Exception exception)
        {
            // Try to recover restart failure
            await RecoverRestartFailure(oldHostState, exception, cancellationToken);
        }

        // If host restarted or recovered successfully, we get here

        Debug.Assert(_currentHost is not null);

        lock (_stateLock)
        {
            // Ensure shutdown handler registered
            RegisterShutdownHandler();

            State = ManagedHostState.Started;

            var hostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

            _restartTaskCompletionSource.SetResult(hostState);
            _restartTaskCompletionSource = new TaskCompletionSource<IRuntimeHostState>();
        }
    }

    private async Task PerformRestart(IRuntimeHostState oldHostState, CancellationToken cancellationToken)
    {
        Debug.Assert(_currentHostBuilderFactory is not null);

        // Get new host builder factory if factory provider is volatile
        var hostBuilderFactory = _hostBuilderFactoryProvider.Volatile
            ? _hostBuilderFactoryProvider.GetHostBuilderFactory()
            : _currentHostBuilderFactory;

        // Try to build new host
        var newHost = BuildHost(hostBuilderFactory);

        // Try to launch new host
        await newHost.StartAsync(cancellationToken);

        // Access new runtime host state
        var newHostState = newHost.Services.GetRequiredService<RuntimeHostState>();
        newHostState.HostRestarted = true;
        newHostState.RestartCompletedAt = DateTime.Now;

        // Copy some properties of old host state to new one
        newHostState.Properties = oldHostState.Properties;
        newHostState.RestartRequestedAt = oldHostState.RestartRequestedAt;

        // Replace old host with new one
        _currentHost = newHost;

        // Replace old host builder factory with new one
        _currentHostBuilderFactory = hostBuilderFactory;

        if (_managedHostHandler is not null)
        {
            await _managedHostHandler.HandleHostTransitionAsync(_currentHost.Services, newHostState, cancellationToken);
        }
    }

    private async Task RecoverRestartFailure(IRuntimeHostState oldHostState, Exception exception,
        CancellationToken cancellationToken)
    {
        Debug.Assert(_shutdownTaskCompletionSource is not null);

        // Finally shutdown host if factory provider is not volatile
        if (!_hostBuilderFactoryProvider.Volatile)
        {
            throw exception;
        }

        Debug.Assert(_currentHostBuilderFactory is not null);

        // Recreate host using old host builder factory
        _currentHost = BuildHost(_currentHostBuilderFactory);
        await _currentHost.StartAsync(cancellationToken);

        var newHostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

        // Report host restart failure
        newHostState.RestartFailed = true;
        newHostState.StartupException = exception;
        newHostState.Properties = oldHostState.Properties;

        if (_managedHostHandler is not null)
        {
            await _managedHostHandler.HandleHostTransitionAsync(_currentHost.Services, newHostState, cancellationToken);
        }
    }

    private async void RegisterShutdownHandler()
    {
        Debug.Assert(_currentHost is not null);

        // Update managed host state when internal host is going to shutdown
        var lifetime = _currentHost.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(UpdateHostState);

        try
        {
            // Wait  for internal host shutdown
            await _currentHost.WaitForShutdownAsync(_shutdownCancellationToken);

            // Invoke shutdown handler
            await HandleHostShutdown(_shutdownCancellationToken);
        }
        catch (Exception exception)
        {
            Debug.Assert(_shutdownTaskCompletionSource is not null);
            Debug.Assert(_restartTaskCompletionSource is not null);

            // Report restart/shutdown failure to all listeners
            var unrecoverableException = new UnrecoverableHostStateException(exception);

            lock (_stateLock)
            {
                State = ManagedHostState.NotStarted;

                _shutdownTaskCompletionSource.SetException(unrecoverableException);
                _shutdownTaskCompletionSource = null;

                _restartTaskCompletionSource.SetException(unrecoverableException);
                _restartTaskCompletionSource = null;
            }
        }
    }

    private void CheckHostState(params ManagedHostState[] expectedStates)
    {
        if (!expectedStates.Contains(State))
        {
            throw new InvalidHostStateException(expectedStates, State);
        }
    }

    private static IHost BuildHost(IHostBuilderFactory hostBuilderFactory)
    {
        return hostBuilderFactory.CreateHostBuilder()
            .ConfigureServices(ServiceCollectionHelper.AddManagedRuntimeServices)
            .Build();
    }

    public static IManagedHostBuilder CreateDefaultBuilder(string[]? args = null)
    {
        return new ManagedHostBuilder().UseHostBuilderFactory(new DefaultHostBuilderFactory(args));
    }
}
