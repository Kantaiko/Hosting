using Kantaiko.Hosting.Modularity.TypeRegistration;
using Kantaiko.Routing.Events;
using Kantaiko.Routing.Handlers;

namespace Kantaiko.Hosting.Lifecycle.TypeRegistration;

public abstract class EventHandlerTypeRegistrationHandler : ITypeRegistrationHandler
{
    public bool Handle(Type type)
    {
        if (!type.IsClass || type.IsAbstract)
        {
            return false;
        }

        var interfaces = type.GetInterfaces();

        foreach (var @interface in interfaces)
        {
            if (!@interface.IsGenericType || @interface.GetGenericTypeDefinition() != typeof(IHandler<,>))
            {
                continue;
            }

            var typeArguments = @interface.GetGenericArguments();

            if (typeArguments[1] != typeof(Task))
            {
                continue;
            }

            return RegisterHandler(typeArguments[0], type);
        }

        return false;
    }

    public virtual void Complete() { }

    protected static AsyncEventHandler<IEventContext<TEvent>> CreateHandler<TEvent>(Type type)
    {
        var handler = new TransientHandler<IEventContext<TEvent>, Task>(type, ServiceHandlerFactory.Instance);

        return handler.Handle;
    }

    protected abstract bool RegisterHandler(Type contextType, Type handlerType);
}
