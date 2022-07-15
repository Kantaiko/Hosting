using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity;

public static class ServiceCollectionExtensions
{
    public static IConfiguration GetConfiguration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return ServiceCollectionHelper.GetHostBuilderContext(services).Configuration;
    }

    public static IHostEnvironment GetHostingEnvironment(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return ServiceCollectionHelper.GetHostBuilderContext(services).HostingEnvironment;
    }

    public static void AddModularity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        ServiceCollectionHelper.TryAddHostInfo(services);
    }

    public static void AddModule<TModule>(this IServiceCollection services) where TModule : IModule
    {
        ArgumentNullException.ThrowIfNull(services);

        var moduleType = typeof(TModule);

        if (!moduleType.IsAssignableTo(typeof(IModule)))
        {
            throw new InvalidOperationException($"Type \"{moduleType.Name}\" is not a valid module type");
        }

        ServiceCollectionHelper.GetModuleManager(services).AddModule(moduleType);
    }

    public static void AddModule(this IServiceCollection services, Type moduleType)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(moduleType);

        ServiceCollectionHelper.GetModuleManager(services).AddModule(moduleType);
    }

    public static bool IsModuleRegistered<TModule>(this IServiceCollection services) where TModule : IModule
    {
        ArgumentNullException.ThrowIfNull(services);

        return ServiceCollectionHelper.GetModuleManager(services).IsRegistered(typeof(TModule));
    }

    public static bool IsModuleRegistered(this IServiceCollection services, Type moduleType)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(moduleType);

        return ServiceCollectionHelper.GetModuleManager(services).IsRegistered(moduleType);
    }

    public static void TryConfigure<TOptions>(this IServiceCollection services, Action<TOptions>? configureOptions)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(services);

        if (configureOptions is not null)
        {
            services.Configure(configureOptions);
        }
    }
}
