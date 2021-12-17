using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modularity;

public interface IModule
{
    ModuleMetadata GetMetadata();

    void ConfigureServices(IServiceCollection services);
}
