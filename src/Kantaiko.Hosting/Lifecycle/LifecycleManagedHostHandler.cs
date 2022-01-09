using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Hosting.Managed;
using Kantaiko.Hosting.Managed.Runtime;
using Kantaiko.Routing.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Lifecycle;

public class LifecycleManagedHostHandler : IManagedHostHandler
{
    public async Task HandleInitialHostStart(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var lifecycle = serviceProvider.GetRequiredService<IManagedHostLifecycle>();

        using var scope = serviceProvider.CreateScope();

        var context = new EventContext<HostInitiallyStartedEvent>(
            new HostInitiallyStartedEvent(),
            scope.ServiceProvider, cancellationToken: cancellationToken);

        await lifecycle.HostInitiallyStarted.Handle(context);
    }

    public async Task HandleHostTransition(IServiceProvider serviceProvider, IRuntimeHostState hostState,
        CancellationToken cancellationToken)
    {
        var lifecycle = serviceProvider.GetRequiredService<IManagedHostLifecycle>();

        using var scope = serviceProvider.CreateScope();

        var context = new EventContext<HostTransitionCompletedEvent>(
            new HostTransitionCompletedEvent(hostState),
            scope.ServiceProvider, cancellationToken: cancellationToken);

        await lifecycle.HostTransitionCompleted.Handle(context);
    }
}
