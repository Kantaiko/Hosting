using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;

namespace Kantaiko.Hosting.Lifecycle.TypeRegistration;

internal class ApplicationLifecycleRegistrationHandler : EventHandlerTypeRegistrationHandler
{
    private readonly IApplicationLifecycle _lifecycle;

    public ApplicationLifecycleRegistrationHandler(IApplicationLifecycle lifecycle)
    {
        _lifecycle = lifecycle;
    }

    protected override bool RegisterHandler(Type contextType, Type handlerType)
    {
        if (contextType == typeof(IAsyncEventContext<ApplicationStartingEvent>))
        {
            _lifecycle.ApplicationStarting +=
                CreateAsyncHandler<IAsyncEventContext<ApplicationStartingEvent>>(handlerType);

            return true;
        }

        if (contextType == typeof(IEventContext<ApplicationStartedEvent>))
        {
            _lifecycle.ApplicationStarted += CreateHandler<IEventContext<ApplicationStartedEvent>>(handlerType);

            return true;
        }

        if (contextType == typeof(IAsyncEventContext<ApplicationStoppingEvent>))
        {
            _lifecycle.ApplicationStopping +=
                CreateAsyncHandler<IAsyncEventContext<ApplicationStoppingEvent>>(handlerType);

            return true;
        }

        if (contextType == typeof(IEventContext<ApplicationStoppedEvent>))
        {
            _lifecycle.ApplicationStopped += CreateHandler<IEventContext<ApplicationStoppedEvent>>(handlerType);

            return true;
        }


        return false;
    }
}
