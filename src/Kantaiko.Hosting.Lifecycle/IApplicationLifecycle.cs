using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public interface IApplicationLifecycle
{
    IHandler<IEventContext<ApplicationStartingEvent>, Task<Unit>> ApplicationStarting { get; set; }
    IHandler<IEventContext<ApplicationStartedEvent>, Task<Unit>> ApplicationStarted { get; set; }

    IHandler<IEventContext<ApplicationStoppingEvent>, Task<Unit>> ApplicationStopping { get; set; }
    IHandler<IEventContext<ApplicationStoppedEvent>, Task<Unit>> ApplicationStopped { get; set; }
}
