using System.Collections.Immutable;
using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Kantaiko.Routing.AutoRegistration;

namespace Kantaiko.Hosting.Lifecycle;

public class ApplicationLifecycle : IApplicationLifecycle
{
    private IHandler<LifecycleEventContext<ApplicationStartingEvent>, Task<Unit>> _applicationStarting;
    private IHandler<LifecycleEventContext<ApplicationStartedEvent>, Task<Unit>> _applicationStarted;
    private IHandler<LifecycleEventContext<ApplicationStoppingEvent>, Task<Unit>> _applicationStopping;
    private IHandler<LifecycleEventContext<ApplicationStoppedEvent>, Task<Unit>> _applicationStopped;

    public ApplicationLifecycle()
    {
        _applicationStarting = Handler.EmptyAsync<LifecycleEventContext<ApplicationStartingEvent>>();
        _applicationStarted = Handler.EmptyAsync<LifecycleEventContext<ApplicationStartedEvent>>();
        _applicationStopping = Handler.EmptyAsync<LifecycleEventContext<ApplicationStoppingEvent>>();
        _applicationStopped = Handler.EmptyAsync<LifecycleEventContext<ApplicationStoppedEvent>>();
    }

    public ApplicationLifecycle(IEnumerable<Type> types)
    {
        var typeCollection = types is ICollection<Type> collection ? collection : types.ToImmutableArray();

        _applicationStarting = Handler.SequentialAsync(HandlerAutoRegistrationService
            .GetTransientHandlers<LifecycleEventContext<ApplicationStartingEvent>, Task<Unit>>(typeCollection,
                ServiceHandlerFactory.Instance));

        _applicationStarted = Handler.ParallelAsync(HandlerAutoRegistrationService
            .GetTransientHandlers<LifecycleEventContext<ApplicationStartedEvent>, Task<Unit>>(typeCollection,
                ServiceHandlerFactory.Instance));

        _applicationStopping = Handler.SequentialAsync(HandlerAutoRegistrationService
            .GetTransientHandlers<LifecycleEventContext<ApplicationStoppingEvent>, Task<Unit>>(typeCollection,
                ServiceHandlerFactory.Instance));

        _applicationStopped = Handler.ParallelAsync(HandlerAutoRegistrationService
            .GetTransientHandlers<LifecycleEventContext<ApplicationStoppedEvent>, Task<Unit>>(typeCollection,
                ServiceHandlerFactory.Instance));
    }

    public ApplicationLifecycle(
        IHandler<LifecycleEventContext<ApplicationStartingEvent>, Task<Unit>> applicationStarting,
        IHandler<LifecycleEventContext<ApplicationStartedEvent>, Task<Unit>> applicationStarted,
        IHandler<LifecycleEventContext<ApplicationStoppingEvent>, Task<Unit>> applicationStopping,
        IHandler<LifecycleEventContext<ApplicationStoppedEvent>, Task<Unit>> applicationStopped)
    {
        _applicationStarting = applicationStarting;
        _applicationStarted = applicationStarted;
        _applicationStopping = applicationStopping;
        _applicationStopped = applicationStopped;
    }

    public IHandler<LifecycleEventContext<ApplicationStartingEvent>, Task<Unit>> ApplicationStarting
    {
        get => _applicationStarting;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStarting = value;
        }
    }

    public IHandler<LifecycleEventContext<ApplicationStartedEvent>, Task<Unit>> ApplicationStarted
    {
        get => _applicationStarted;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStarted = value;
        }
    }

    public IHandler<LifecycleEventContext<ApplicationStoppingEvent>, Task<Unit>> ApplicationStopping
    {
        get => _applicationStopping;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStopping = value;
        }
    }

    public IHandler<LifecycleEventContext<ApplicationStoppedEvent>, Task<Unit>> ApplicationStopped
    {
        get => _applicationStopped;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _applicationStopped = value;
        }
    }
}
