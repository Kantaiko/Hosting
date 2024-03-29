﻿using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Lifecycle.TypeRegistration;

internal class ManagedHostLifecycleRegistrationHandler : EventHandlerTypeRegistrationHandler
{
    private readonly ManagedHostLifecycle _lifecycle;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public ManagedHostLifecycleRegistrationHandler(ManagedHostLifecycle lifecycle,
        IHostApplicationLifetime applicationLifetime)
    {
        _lifecycle = lifecycle;
        _applicationLifetime = applicationLifetime;
    }

    protected override bool RegisterHandler(Type contextType, Type handlerType)
    {
        if (contextType == typeof(IAsyncEventContext<HostInitiallyStartedEvent>))
        {
            _lifecycle.HostInitiallyStarted +=
                CreateAsyncHandler<IAsyncEventContext<HostInitiallyStartedEvent>>(handlerType);

            return true;
        }

        if (contextType == typeof(IAsyncEventContext<HostTransitionCompletedEvent>))
        {
            _lifecycle.HostTransitionCompleted +=
                CreateAsyncHandler<IAsyncEventContext<HostTransitionCompletedEvent>>(handlerType);

            return true;
        }

        return false;
    }

    public override void Complete()
    {
        _applicationLifetime.ApplicationStopping.Register(_lifecycle.ClearHandlers);
    }
}
