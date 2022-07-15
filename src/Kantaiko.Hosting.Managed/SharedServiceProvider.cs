using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Managed;

public class SharedServiceProvider : ISharedServiceProvider
{
    private readonly ConcurrentDictionary<Type, object> _services = new();

    public void SetService(Type serviceType, object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        _services[serviceType] = instance;
    }

    public object GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return _services.GetOrAdd(serviceType, key => ActivatorUtilities.CreateInstance(this, key));
    }

    public object GetRequiredService(Type serviceType, Func<IServiceProvider, object> factory)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(factory);

        return _services.GetOrAdd(serviceType, _ => factory(this));
    }
}
