using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Host;

public class DefaultHostLoaderHandler : IHostLoaderHandler
{
    public virtual void Handle(LoadedHost loadedHost, IServiceCollection serviceCollection) { }

    private static DefaultHostLoaderHandler? _instance;
    public static DefaultHostLoaderHandler Instance => _instance ??= new DefaultHostLoaderHandler();
}
