using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public interface IManagedHostBuilder
{
    IModuleCollection Modules { get; }
    void UseHostLoaderHandler(IHostLoaderHandler hostLoaderHandler);
    void UseHostBuilderFactory(IHostBuilderFactory hostBuilderFactory);
    void ConfigureHostBuilder(Action<IHostBuilder> configureDelegate);
    void UseHostLifecycleHandler(IHostLifecycleHandler hostLifecycleHandler);
    IManagedHost Build();
}
