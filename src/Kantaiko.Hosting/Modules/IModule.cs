using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modules;

public interface IModule
{
    void ConfigureModules(IModuleCollection modules) { }
    void ConfigureServices(IServiceCollection services) { }
}