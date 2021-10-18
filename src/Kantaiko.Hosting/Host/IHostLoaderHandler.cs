using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Host;

public interface IHostLoaderHandler
{
    /// <summary>
    /// Invokes during the DI configuration stage after loading the host and their modules.
    /// Exposes constructed module instances for advanced service customization.
    /// </summary>
    /// <param name="loadedHost"></param>
    /// <param name="serviceCollection"></param>
    /// <returns></returns>
    void Handle(LoadedHost loadedHost, IServiceCollection serviceCollection);
}
