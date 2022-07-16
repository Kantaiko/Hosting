using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public interface IApplicationLifecycle
{
    event AsyncEventHandler<IAsyncEventContext<ApplicationStartingEvent>> ApplicationStarting;
    event SyncEventHandler<IEventContext<ApplicationStartedEvent>> ApplicationStarted;

    event AsyncEventHandler<IAsyncEventContext<ApplicationStoppingEvent>> ApplicationStopping;
    event SyncEventHandler<IEventContext<ApplicationStoppedEvent>> ApplicationStopped;
}
