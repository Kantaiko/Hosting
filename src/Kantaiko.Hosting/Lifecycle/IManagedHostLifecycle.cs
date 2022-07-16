using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public interface IManagedHostLifecycle
{
    event AsyncEventHandler<IAsyncEventContext<HostInitiallyStartedEvent>> HostInitiallyStarted;
    event AsyncEventHandler<IAsyncEventContext<HostTransitionCompletedEvent>> HostTransitionCompleted;
}
