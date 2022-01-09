using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Kantaiko.Routing.AutoRegistration;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public class ManagedHostLifecycle : IManagedHostLifecycle
{
    private IHandler<IEventContext<HostInitiallyStartedEvent>, Task<Unit>> _hostInitiallyStarted;
    private IHandler<IEventContext<HostTransitionCompletedEvent>, Task<Unit>> _hostTransitionCompleted;

    public ManagedHostLifecycle()
    {
        _hostInitiallyStarted = Handler.EmptyAsync<IEventContext<HostInitiallyStartedEvent>>();
        _hostTransitionCompleted = Handler.EmptyAsync<IEventContext<HostTransitionCompletedEvent>>();
    }

    public ManagedHostLifecycle(IEnumerable<Type> types)
    {
        types = types.Concat(typeof(ManagedHostLifecycle).Assembly.GetTypes());
        var typeCollection = AutoRegistrationUtils.MaterializeCollection(types);

        _hostInitiallyStarted = EventHandlerFactory
            .CreateSequentialEventHandler<HostInitiallyStartedEvent>(typeCollection, ServiceHandlerFactory.Instance);

        _hostTransitionCompleted = EventHandlerFactory
            .CreateSequentialEventHandler<HostTransitionCompletedEvent>(typeCollection, ServiceHandlerFactory.Instance);
    }

    public ManagedHostLifecycle(
        IHandler<IEventContext<HostInitiallyStartedEvent>, Task<Unit>> hostInitiallyStarted,
        IHandler<IEventContext<HostTransitionCompletedEvent>, Task<Unit>> hostTransitionCompleted)
    {
        _hostInitiallyStarted = hostInitiallyStarted;
        _hostTransitionCompleted = hostTransitionCompleted;
    }

    public IHandler<IEventContext<HostInitiallyStartedEvent>, Task<Unit>> HostInitiallyStarted
    {
        get => _hostInitiallyStarted;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _hostInitiallyStarted = value;
        }
    }

    public IHandler<IEventContext<HostTransitionCompletedEvent>, Task<Unit>> HostTransitionCompleted
    {
        get => _hostTransitionCompleted;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _hostTransitionCompleted = value;
        }
    }
}
