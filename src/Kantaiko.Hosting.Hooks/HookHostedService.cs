using Kantaiko.Hosting.Hooks.ApplicationHooks;
using Kantaiko.Hosting.Introspection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Hooks;

internal class HookHostedService : IHostedService
{
    private readonly HookInitializer _hookInitializer;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IHookDispatcher _hookDispatcher;
    private readonly HostInfo _hostInfo;

    public HookHostedService(HookInitializer hookInitializer, IHostApplicationLifetime applicationLifetime,
        IHookDispatcher hookDispatcher, HostInfo hostInfo)
    {
        _hookInitializer = hookInitializer;
        _applicationLifetime = applicationLifetime;
        _hookDispatcher = hookDispatcher;
        _hostInfo = hostInfo;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _hookInitializer.Initialize(_hostInfo.Assemblies);

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
