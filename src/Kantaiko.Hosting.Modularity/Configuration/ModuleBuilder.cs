using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modularity.Configuration;

public abstract class ModuleBuilder
{
    protected ModuleBuilder(IServiceCollection services)
    {
        Services = services;
    }

    protected IServiceCollection Services { get; }
}
