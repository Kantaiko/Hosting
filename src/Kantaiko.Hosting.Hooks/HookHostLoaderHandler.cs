using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Hooks;

public class HookHostLoaderHandler : DefaultHostLoaderHandler
{
    public override void Handle(LoadedHost loadedHost, IServiceCollection serviceCollection)
    {
        serviceCollection.AddHookServices();
    }

    private static HookHostLoaderHandler? _instance;
    public new static HookHostLoaderHandler Instance => _instance ??= new HookHostLoaderHandler();
}
