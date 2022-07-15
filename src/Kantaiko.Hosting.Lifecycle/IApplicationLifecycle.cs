using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public interface IApplicationLifecycle
{
    event AsyncEventHandler<IEventContext<ApplicationStartingEvent>> ApplicationStarting;
    event AsyncEventHandler<IEventContext<ApplicationStartedEvent>> ApplicationStarted;

    event AsyncEventHandler<IEventContext<ApplicationStoppingEvent>> ApplicationStopping;
    event AsyncEventHandler<IEventContext<ApplicationStoppedEvent>> ApplicationStopped;
}
