using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;

namespace Kantaiko.Hosting.Lifecycle;

public interface IApplicationLifecycle
{
    IHandler<LifecycleEventContext<ApplicationStartingEvent>, Task<Unit>> ApplicationStarting { get; set; }
    IHandler<LifecycleEventContext<ApplicationStartedEvent>, Task<Unit>> ApplicationStarted { get; set; }

    IHandler<LifecycleEventContext<ApplicationStoppingEvent>, Task<Unit>> ApplicationStopping { get; set; }
    IHandler<LifecycleEventContext<ApplicationStoppedEvent>, Task<Unit>> ApplicationStopped { get; set; }
}
