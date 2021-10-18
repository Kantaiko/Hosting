using System.Diagnostics;
using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

/// <inheritdoc />
public class ManagedHost : IManagedHost
{
    private readonly IHostConstructionContextProvider _hostConstructionContextProvider;
    private readonly IHostLoaderHandler _hostLoaderHandler;
    private readonly IHostLifecycleHandler _hostLifecycleHandler;
    private readonly IHostBuilderFactory _hostBuilderFactory;

    private TaskCompletionSource? _shutdownTaskCompletionSource;
    private TaskCompletionSource<IRuntimeHostState>? _restartTaskCompletionSource;

    private CancellationTokenSource? _shutdownTokenSource;

    public ManagedHostState State { get; private set; } = ManagedHostState.NotStarted;

    public bool IsStarted => State == ManagedHostState.Started;

    private HostConstructionContext? _currentHostConstructionContext;
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

    public ManagedHost(IHostConstructionContextProvider? hostConstructionContextProvider = null,
        IHostLoaderHandler? hostLoaderHandler = null,
        IHostLifecycleHandler? hostLifecycleHandler = null,
        IHostBuilderFactory? hostBuilderFactory = null)
    {
        _hostConstructionContextProvider =
            hostConstructionContextProvider ?? DefaultHostConstructionContextProvider.Instance;
        _hostLoaderHandler = hostLoaderHandler ?? DefaultHostLoaderHandler.Instance;
        _hostLifecycleHandler = hostLifecycleHandler ?? DefaultHostLifecycleHandler.Instance;
        _hostBuilderFactory = hostBuilderFactory ?? DefaultHostBuilderFactory.Instance;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.NotStarted);
            State = ManagedHostState.Starting;
        }

        // Create new shutdown completion source
        _shutdownTaskCompletionSource = new TaskCompletionSource();
        _restartTaskCompletionSource = new TaskCompletionSource<IRuntimeHostState>();

        try
        {
            // Build new host
            _currentHostConstructionContext = _hostConstructionContextProvider.GetHostConstructionContext();
            _currentHost = BuildHost(_currentHostConstructionContext);


            // Start current host
            await _currentHost.StartAsync(cancellationToken);

            _shutdownTokenSource = new CancellationTokenSource();

            // Register shutdown handler to perform host restart
            RegisterShutdownHandler(_shutdownTokenSource.Token);

            State = ManagedHostState.Started;
        }
        catch
        {
            _shutdownTaskCompletionSource.SetCanceled(cancellationToken);
            _restartTaskCompletionSource.SetCanceled(cancellationToken);

            _shutdownTaskCompletionSource = null;
            _restartTaskCompletionSource = null;

            State = ManagedHostState.NotStarted;

            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started);
            State = ManagedHostState.Stopping;
        }

        Debug.Assert(_currentHost is not null);

        try
        {
            // Stop current host
            await _currentHost.StopAsync(cancellationToken);
            await WaitForShutdownAsync(cancellationToken);
        }
        finally
        {
            // Dispose host
            _currentHost.Dispose();
            _currentHost = null;

            _shutdownTaskCompletionSource = null;
            _restartTaskCompletionSource = null;

            State = ManagedHostState.NotStarted;
        }
    }

    public async Task<IRuntimeHostState> RestartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started);
            State = ManagedHostState.Restarting;
        }

        Debug.Assert(_currentHost is not null);

        var runtimeHostManager = _currentHost.Services.GetRequiredService<IRuntimeHostManager>();
        runtimeHostManager.Restart();

        return await WaitForRestartAsync(cancellationToken);
    }

    public Task WaitForShutdownAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started, ManagedHostState.Stopping);
        }

        Debug.Assert(_shutdownTaskCompletionSource is not null);

        var taskCompletionSource = new TaskCompletionSource();

        _shutdownTaskCompletionSource.Task.ContinueWith(_ => taskCompletionSource.SetResult(), cancellationToken);
        cancellationToken.Register(taskCompletionSource.SetCanceled);

        return taskCompletionSource.Task;
    }

    public Task<IRuntimeHostState> WaitForRestartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            CheckHostState(ManagedHostState.Started, ManagedHostState.Restarting);
        }

        Debug.Assert(_restartTaskCompletionSource is not null);

        var taskCompletionSource = new TaskCompletionSource<IRuntimeHostState>();
        _restartTaskCompletionSource.Task.ContinueWith(task => taskCompletionSource.SetResult(task.Result),
            cancellationToken);

        cancellationToken.Register(taskCompletionSource.SetCanceled);

        return taskCompletionSource.Task;
    }

    public void Dispose()
    {
        _currentHost?.Dispose();
    }

    private async Task HandleHostShutdown(CancellationToken cancellationToken)
    {
        Debug.Assert(_currentHost is not null);
        Debug.Assert(_currentHostConstructionContext is not null);

        // Get old host state
        var oldHostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

        // Dispose old host
        _currentHost.Dispose();

        if (!oldHostState.RestartRequested)
        {
            // Restart was not requested, so host is going to shutdown
            Debug.Assert(_shutdownTaskCompletionSource is not null);
            _shutdownTaskCompletionSource.SetResult();
            return;
        }

        try
        {
            var newHostConstructionContext = _hostConstructionContextProvider.GetHostConstructionContext();

            // Try to build new host using new host builder
            var newHost = BuildHost(newHostConstructionContext);

            // Try to launch new host
            await newHost.StartAsync(cancellationToken);

            var newHostState = newHost.Services.GetRequiredService<RuntimeHostState>();
            newHostState.HostRestarted = true;
            newHostState.RestartCompletedAt = DateTime.Now;

            // Copy some properties of old host state to new one
            newHostState.Properties = oldHostState.Properties;
            newHostState.RestartRequestedAt = oldHostState.RestartRequestedAt;

            // Replace old host and construction context with new ones
            _currentHost = newHost;
            _currentHostConstructionContext = newHostConstructionContext;

            // Invoke restart handler
            await _hostLifecycleHandler.HandleRestartAsync(_currentHost.Services, cancellationToken);
        }
        catch (Exception exception)
        {
            // Recreate host using old construction context
            _currentHost = BuildHost(_currentHostConstructionContext);
            await _currentHost.StartAsync(cancellationToken);

            // Report host restart failure
            var hostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();
            hostState.RestartFailed = true;
            hostState.StartupException = exception;
            hostState.Properties = oldHostState.Properties;

            // Invoke restart fail handler
            await _hostLifecycleHandler.HandleRestartFailAsync(_currentHost.Services, exception, cancellationToken);
        }
        finally
        {
            Debug.Assert(_shutdownTokenSource is not null);

            // Ensure shutdown handler registered
            RegisterShutdownHandler(_shutdownTokenSource.Token);

            var hostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

            State = ManagedHostState.Started;

            Debug.Assert(_restartTaskCompletionSource is not null);

            _restartTaskCompletionSource.SetResult(hostState);
            _restartTaskCompletionSource = new TaskCompletionSource<IRuntimeHostState>();
        }
    }

    private IHost BuildHost(HostConstructionContext constructionContext)
    {
        var builder = _hostBuilderFactory.CreateHostBuilder(constructionContext);

        builder.ConfigureKantaikoHosting(constructionContext, _hostLoaderHandler);

        return builder.Build();
    }

    private async void RegisterShutdownHandler(CancellationToken cancellationToken)
    {
        await _currentHost.WaitForShutdownAsync(cancellationToken);
        await HandleHostShutdown(cancellationToken);
    }

    private void CheckHostState(params ManagedHostState[] expectedStates)
    {
        if (!expectedStates.Contains(State))
        {
            throw new InvalidHostStateException(expectedStates, State);
        }
    }
}
