using Kantaiko.Hosting.Hooks.HostHooks;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Hooks;

public class HookHostLifecycleHandler : DefaultHostLifecycleHandler
{
    public override async Task HandleRestartAsync(IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var hookDispatcher = serviceProvider.GetRequiredService<IHookDispatcher>();

        var hostState = serviceProvider.GetRequiredService<IRuntimeHostState>();

        var applicationRestartedHook = new ManagedHostRestartedHook(hostState);
        await hookDispatcher.DispatchAsync(applicationRestartedHook, cancellationToken);
    }

    public override async Task HandleRestartFailAsync(IServiceProvider serviceProvider, Exception exception,
        CancellationToken cancellationToken = default)
    {
        var hookDispatcher = serviceProvider.GetRequiredService<IHookDispatcher>();

        var hostState = serviceProvider.GetRequiredService<IRuntimeHostState>();

        var applicationRestartedHook = new ManagedHostRestartFailedHook(hostState);
        await hookDispatcher.DispatchAsync(applicationRestartedHook, cancellationToken);
    }

    private static HookHostLifecycleHandler? _instance;
    public new static HookHostLifecycleHandler Instance => _instance ??= new HookHostLifecycleHandler();
}
