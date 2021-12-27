using Kantaiko.Hosting.Lifecycle.Events;

namespace Kantaiko.Hosting.Lifecycle;

public abstract class LifecycleEventHandler<TEvent> : Routing.Events.EventHandler<TEvent>
    where TEvent : IApplicationLifecycleEvent { }
