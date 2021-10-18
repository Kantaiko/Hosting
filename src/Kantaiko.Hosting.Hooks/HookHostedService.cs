using Kantaiko.Hosting.Hooks.ApplicationHooks;
using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Hooks;

internal class HookHostedService : IHostedService
{
    private readonly HookInitializer _hookInitializer;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IHookDispatcher _hookDispatcher;
    private readonly LoadedHost _loadedHost;

    public HookHostedService(HookInitializer hookInitializer, IHostApplicationLifetime applicationLifetime,
        IHookDispatcher hookDispatcher, LoadedHost loadedHost)
    {
        _hookInitializer = hookInitializer;
        _applicationLifetime = applicationLifetime;
        _hookDispatcher = hookDispatcher;
        _loadedHost = loadedHost;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _hookInitializer.Initialize(_loadedHost.HostInfo.Assemblies);

        var applicationStartupHook = new ApplicationStartupHook();
        await _hookDispatcher.DispatchAsync(applicationStartupHook, cancellationToken);

        _applicationLifetime.ApplicationStarted.Register(() =>
        {
            var applicationReadyHook = new ApplicationReadyHook();
            _hookDispatcher.DispatchAsync(applicationReadyHook, cancellationToken);
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var applicationShutdownHook = new ApplicationShutdownHook();
        await _hookDispatcher.DispatchAsync(applicationShutdownHook, cancellationToken);
    }
}
