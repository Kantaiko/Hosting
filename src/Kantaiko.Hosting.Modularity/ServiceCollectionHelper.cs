using Kantaiko.Hosting.Modularity.Internal;
using Kantaiko.Hosting.Modularity.Introspection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity;

internal static class ServiceCollectionHelper
{
    public static HostBuilderContext GetHostBuilderContext(IServiceCollection services)
    {
        var hostBuilderContext = services.FirstOrDefault(x => x.ServiceType == typeof(HostBuilderContext));

        if (hostBuilderContext?.ImplementationInstance is null)
        {
            throw new InvalidOperationException("Host builder context inaccessible");
        }

        return (HostBuilderContext) hostBuilderContext.ImplementationInstance;
    }

    public static ModuleManager GetModuleManager(IServiceCollection services)
    {
        var moduleManagerDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ModuleManager));

        if (moduleManagerDescriptor is not null)
        {
            return (ModuleManager) moduleManagerDescriptor.ImplementationInstance!;
        }

        var moduleManager = new ModuleManager(services);

        services.AddSingleton(moduleManager);

        services.AddSingleton<HostInfo>(_ => throw new InvalidOperationException(
            "Host info has not been configured. " +
            "Did you forget to call services.CompleteModularityConfiguration() before creating service provider?"));

        return moduleManager;
    }
}
