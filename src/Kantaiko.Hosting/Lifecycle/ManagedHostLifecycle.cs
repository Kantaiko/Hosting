using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Hosting.Managed;
using Kantaiko.Hosting.Managed.Runtime;
using Kantaiko.Routing.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Lifecycle;

internal class ManagedHostLifecycle : IManagedHostLifecycle, IManagedHostHandler
{
    public event AsyncEventHandler<IEventContext<HostInitiallyStartedEvent>>? HostInitiallyStarted;
    public event AsyncEventHandler<IEventContext<HostTransitionCompletedEvent>>? HostTransitionCompleted;

    public async Task HandleInitialHostStart(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = new EventContext<HostInitiallyStartedEvent>(
            new HostInitiallyStartedEvent(),
            scope.ServiceProvider,
            cancellationToken
        );

        await HostInitiallyStarted.InvokeAsync(context);
    }

    public async Task HandleHostTransition(IServiceProvider serviceProvider, IRuntimeHostState hostState,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = new EventContext<HostTransitionCompletedEvent>(
            new HostTransitionCompletedEvent(hostState),
            scope.ServiceProvider,
            cancellationToken
        );

        await HostTransitionCompleted.InvokeAsync(context);
    }

    public void ClearHandlers()
    {
        HostInitiallyStarted = null;
        HostTransitionCompleted = null;
    }
}
