using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modularity.Internal;

internal class ModuleManagerAccessor
{
    public ModuleManager? ModuleManager { get; set; }

    public ModuleManagerAccessor(IServiceCollection services)
    {
        ModuleManager = new ModuleManager(services);
    }
}
