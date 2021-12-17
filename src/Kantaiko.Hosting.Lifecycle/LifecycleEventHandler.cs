using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Kantaiko.Routing.AutoRegistration;

namespace Kantaiko.Hosting.Lifecycle;

public abstract class LifecycleEventHandler<TEvent> : IHandler<LifecycleEventContext<TEvent>, Task<Unit>>,
    IAutoRegistrableHandler
    where TEvent : IApplicationLifecycleEvent
{
    public abstract Task<Unit> Handle(LifecycleEventContext<TEvent> context);
}
