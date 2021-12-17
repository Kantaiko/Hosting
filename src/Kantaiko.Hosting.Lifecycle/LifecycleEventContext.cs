using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Context;

namespace Kantaiko.Hosting.Lifecycle;

public class LifecycleEventContext<TEvent> : ContextBase where TEvent : IApplicationLifecycleEvent
{
    public LifecycleEventContext(TEvent @event, IServiceProvider serviceProvider, CancellationToken cancellationToken) :
        base(serviceProvider, cancellationToken)
    {
        Event = @event;
    }

    public TEvent Event { get; }
}
