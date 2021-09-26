using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Hooks;
using Kantaiko.Hosting.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host
{
    /// <inheritdoc />
    public class ManagedHost : IManagedHost
    {
        private readonly string[]? _args;
        private readonly IHostConstructionContextProvider _hostConstructionContextProvider;
        private readonly IHostModuleHandler? _hostModuleHandler;

        private TaskCompletionSource? _shutdownTaskCompletionSource;
        private TaskCompletionSource<IRuntimeHostState>? _restartTaskCompletionSource;

        public ManagedHostState State { get; private set; } = ManagedHostState.NotStarted;

        public bool IsStarted => State == ManagedHostState.Started;

        private HostConstructionContext? _currentHostConstructionContext;
        private IHost? _currentHost;

        private readonly object _stateLock = new();

        public ManagedHost(string[]? args = null,
            IHostConstructionContextProvider? hostConstructionContextProvider = null,
            IHostModuleHandler? hostModuleHandler = null)
        {
            _args = args;
            _hostConstructionContextProvider =
                hostConstructionContextProvider ?? new DefaultHostConstructionContextProvider();
            _hostModuleHandler = hostModuleHandler;
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

            // Build new host
            _currentHostConstructionContext = _hostConstructionContextProvider.GetHostConstructionContext();
            _currentHost = BuildHost(_currentHostConstructionContext);

            // Start current host
            await _currentHost.StartAsync(cancellationToken);

            // Register shutdown handler to perform host restart
            RegisterShutdownHandler();

            State = ManagedHostState.Started;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            lock (_stateLock)
            {
                CheckHostState(ManagedHostState.Started);
                State = ManagedHostState.Stopping;
            }

            Debug.Assert(_currentHost is not null);

            // Stop current host
            await _currentHost.StopAsync(cancellationToken);
            await WaitForShutdownAsync(cancellationToken);

            // Dispose it
            _currentHost.Dispose();
            _currentHost = null;

            _shutdownTaskCompletionSource = null;
            _restartTaskCompletionSource = null;

            State = ManagedHostState.NotStarted;
        }

        public async Task<IRuntimeHostState> RestartAsync(CancellationToken cancellationToken = default)
        {
            lock (_stateLock)
            {
                CheckHostState(ManagedHostState.Started);
                State = ManagedHostState.Restarting;
            }

            Debug.Assert(Services is not null);

            var runtimeHostManager = Services.GetRequiredService<IRuntimeHostManager>();
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

        public IServiceProvider? Services => _currentHost?.Services;

        public void Dispose()
        {
            _currentHost?.Dispose();
        }

        private async Task HandleHostShutdown()
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
                await newHost.StartAsync();

                var newHostState = newHost.Services.GetRequiredService<RuntimeHostState>();
                newHostState.HostRestarted = true;
                newHostState.RestartCompletedAt = DateTime.Now;

                // Copy some properties of old host state to new one
                newHostState.Properties = oldHostState.Properties;
                newHostState.RestartRequestedAt = oldHostState.RestartRequestedAt;

                // Replace old host and construction context with new ones
                _currentHost = newHost;
                _currentHostConstructionContext = newHostConstructionContext;

                // Dispatch application restarted hook
                var hookDispatcher = _currentHost.Services.GetRequiredService<IHookDispatcher>();
                var applicationRestartedHook = new ApplicationRestartedHook(newHostState);
                await hookDispatcher.DispatchAsync(applicationRestartedHook);
            }
            catch (Exception exception)
            {
                // Recreate host using old construction context
                _currentHost = BuildHost(_currentHostConstructionContext);
                await _currentHost.StartAsync();

                // Report host restart failure
                var hostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();
                hostState.RestartFailed = true;
                hostState.StartupException = exception;
                hostState.Properties = oldHostState.Properties;

                // Dispatch application restart failed hook
                var hookDispatcher = _currentHost.Services.GetRequiredService<IHookDispatcher>();
                var applicationRestartFailedHook = new ApplicationRestartFailedHook(hostState);
                await hookDispatcher.DispatchAsync(applicationRestartFailedHook);
            }
            finally
            {
                // Ensure shutdown handler registered
                RegisterShutdownHandler();

                var hostState = _currentHost.Services.GetRequiredService<RuntimeHostState>();

                State = ManagedHostState.Started;

                Debug.Assert(_restartTaskCompletionSource is not null);
                _restartTaskCompletionSource.SetResult(hostState);
                _restartTaskCompletionSource = new TaskCompletionSource<IRuntimeHostState>();
            }
        }

        private IHost BuildHost(HostConstructionContext constructionContext)
        {
            var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(_args);

            builder.ConfigureKantaikoHosting(constructionContext, _hostModuleHandler);

            return builder.Build();
        }

        private async void RegisterShutdownHandler()
        {
            await _currentHost.WaitForShutdownAsync();
            await HandleHostShutdown();
        }

        private void CheckHostState(params ManagedHostState[] expectedStates)
        {
            if (!expectedStates.Contains(State))
            {
                throw new InvalidHostStateException(expectedStates, State);
            }
        }
    }
}
