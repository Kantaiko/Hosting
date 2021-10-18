using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Host;

public class CombinedHostLoaderHandler : IHostLoaderHandler
{
    private readonly IEnumerable<IHostLoaderHandler> _handlers;

    public CombinedHostLoaderHandler(IEnumerable<IHostLoaderHandler> handlers)
    {
        _handlers = handlers;
    }

    public void Handle(LoadedHost loadedHost, IServiceCollection serviceCollection)
    {
        foreach (var hostLoaderHandler in _handlers)
        {
            hostLoaderHandler.Handle(loadedHost, serviceCollection);
        }
    }
}
