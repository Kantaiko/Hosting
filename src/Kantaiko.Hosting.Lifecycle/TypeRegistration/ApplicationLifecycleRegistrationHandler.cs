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
        if (contextType == typeof(IEventContext<ApplicationStartingEvent>))
        {
            _lifecycle.ApplicationStarting += CreateHandler<ApplicationStartingEvent>(handlerType);
            return true;
        }

        if (contextType == typeof(IEventContext<ApplicationStartedEvent>))
        {
            _lifecycle.ApplicationStarted += CreateHandler<ApplicationStartedEvent>(handlerType);
            return true;
        }

        if (contextType == typeof(IEventContext<ApplicationStoppingEvent>))
        {
            _lifecycle.ApplicationStopping += CreateHandler<ApplicationStoppingEvent>(handlerType);
            return true;
        }

        if (contextType == typeof(IEventContext<ApplicationStoppedEvent>))
        {
            _lifecycle.ApplicationStopped += CreateHandler<ApplicationStoppedEvent>(handlerType);
            return true;
        }


        return false;
    }
}
