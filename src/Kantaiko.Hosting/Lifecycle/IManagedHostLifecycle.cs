using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public interface IManagedHostLifecycle
{
    event AsyncEventHandler<IEventContext<HostInitiallyStartedEvent>> HostInitiallyStarted;
    event AsyncEventHandler<IEventContext<HostTransitionCompletedEvent>> HostTransitionCompleted;
}
