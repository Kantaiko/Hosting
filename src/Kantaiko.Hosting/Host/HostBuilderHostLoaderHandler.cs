using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Host;

public class HostBuilderHostLoaderHandler : IHostLoaderHandler
{
    public void Handle(LoadedHost loadedHost, IServiceCollection serviceCollection) { }
}
