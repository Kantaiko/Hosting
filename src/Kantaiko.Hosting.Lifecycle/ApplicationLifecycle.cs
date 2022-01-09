using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Kantaiko.Routing.AutoRegistration;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle;

public class ApplicationLifecycle : IApplicationLifecycle
{
    private IHandler<IEventContext<ApplicationStartingEvent>, Task<Unit>> _applicationStarting;
    private IHandler<IEventContext<ApplicationStartedEvent>, Task<Unit>> _applicationStarted;
    private IHandler<IEventContext<ApplicationStoppingEvent>, Task<Unit>> _applicationStopping;
    private IHandler<IEventContext<ApplicationStoppedEvent>, Task<Unit>> _applicationStopped;

    public ApplicationLifecycle()
    {
        _applicationStarting = Handler.EmptyAsync<IEventContext<ApplicationStartingEvent>>();
        _applicationStarted = Handler.EmptyAsync<IEventContext<ApplicationStartedEvent>>();
        _applicationStopping = Handler.EmptyAsync<IEventContext<ApplicationStoppingEvent>>();
        _applicationStopped = Handler.EmptyAsync<IEventContext<ApplicationStoppedEvent>>();
    }

    public ApplicationLifecycle(IEnumerable<Type> types)
    {
        types = types.Concat(typeof(ApplicationLifecycle).Assembly.GetTypes());
        var typeCollection = AutoRegistrationUtils.MaterializeCollection(types);

        _applicationStarting = EventHandlerFactory
            .CreateSequentialEventHandler<ApplicationStartingEvent>(typeCollection, ServiceHandlerFactory.Instance);

        _applicationStarted = EventHandlerFactory
            .CreateParallelEventHandler<ApplicationStartedEvent>(typeCollection, ServiceHandlerFactory.Instance);

        _applicationStopping = EventHandlerFactory
            .CreateSequentialEventHandler<ApplicationStoppingEvent>(typeCollection, ServiceHandlerFactory.Instance);

        _applicationStopped = EventHandlerFactory
            .CreateParallelEventHandler<ApplicationStoppedEvent>(typeCollection, ServiceHandlerFactory.Instance);
    }

    public ApplicationLifecycle(
        IHandler<IEventContext<ApplicationStartingEvent>, Task<Unit>> applicationStarting,
        IHandler<IEventContext<ApplicationStartedEvent>, Task<Unit>> applicationStarted,
        IHandler<IEventContext<ApplicationStoppingEvent>, Task<Unit>> applicationStopping,
        IHandler<IEventContext<ApplicationStoppedEvent>, Task<Unit>> applicationStopped)
    {
        _applicationStarting = applicationStarting;
        _applicationStarted = applicationStarted;
        _applicationStopping = applicationStopping;
        _applicationStopped = applicationStopped;
    }

    public IHandler<IEventContext<ApplicationStartingEvent>, Task<Unit>> ApplicationStarting
    {
        get => _applicationStarting;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStarting = value;
        }
    }

    public IHandler<IEventContext<ApplicationStartedEvent>, Task<Unit>> ApplicationStarted
    {
        get => _applicationStarted;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStarted = value;
        }
    }

    public IHandler<IEventContext<ApplicationStoppingEvent>, Task<Unit>> ApplicationStopping
    {
        get => _applicationStopping;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStopping = value;
        }
    }

    public IHandler<IEventContext<ApplicationStoppedEvent>, Task<Unit>> ApplicationStopped
    {
        get => _applicationStopped;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStopped = value;
        }
    }
}
