using System.Diagnostics;
using Kantaiko.Hosting.Modularity.Internal;
using Kantaiko.Hosting.Modularity.Introspection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        var moduleManagerAccessorDescriptor =
            services.FirstOrDefault(x => x.ServiceType == typeof(ModuleManagerAccessor));

        ModuleManagerAccessor moduleManagerAccessor;

        if (moduleManagerAccessorDescriptor is not null)
        {
            moduleManagerAccessor = (ModuleManagerAccessor) moduleManagerAccessorDescriptor.ImplementationInstance!;

            Debug.Assert(moduleManagerAccessor.ModuleManager is not null);

            return moduleManagerAccessor.ModuleManager;
        }

        moduleManagerAccessor = new ModuleManagerAccessor(services);

        services.AddSingleton(moduleManagerAccessor);
        TryAddHostInfo(services);

        return moduleManagerAccessor.ModuleManager!;
    }

    public static void TryAddHostInfo(IServiceCollection services)
    {
        services.TryAddSingleton(provider =>
        {
            var moduleManagerAccessor = provider.GetService<ModuleManagerAccessor>();

            if (moduleManagerAccessor?.ModuleManager is not { } moduleManager)
            {
                return new HostInfo();
            }

            var hostInfoFactory = new HostInfoFactory(moduleManager);
            var hostInfo = hostInfoFactory.CreateHostInfo();

            // Release temporary module manager infrastructure
            moduleManagerAccessor.ModuleManager = null;

            return hostInfo;
        });
    }
}
