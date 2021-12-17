using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modularity;

public abstract class Module : IModule
{
    ModuleMetadata IModule.GetMetadata()
    {
        return ModuleMetadataExtractor.Extract(this);
    }

    protected virtual void ConfigureServices(IServiceCollection services) { }

    void IModule.ConfigureServices(IServiceCollection services)
    {
        ConfigureServices(services);
    }
}
