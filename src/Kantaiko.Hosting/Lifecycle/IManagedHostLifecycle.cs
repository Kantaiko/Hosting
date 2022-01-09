using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public interface IManagedHostLifecycle
{
    IHandler<IEventContext<HostInitiallyStartedEvent>, Task<Unit>> HostInitiallyStarted { get; set; }
    IHandler<IEventContext<HostTransitionCompletedEvent>, Task<Unit>> HostTransitionCompleted { get; set; }
}
