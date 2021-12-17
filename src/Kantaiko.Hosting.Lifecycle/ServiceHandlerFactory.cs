using Kantaiko.Routing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Lifecycle;

public class ServiceHandlerFactory : IHandlerFactory
{
    public object CreateHandler(Type handlerType, IServiceProvider serviceProvider)
    {
        return ActivatorUtilities.CreateInstance(serviceProvider, handlerType);
    }

    public static ServiceHandlerFactory Instance { get; } = new();
}
