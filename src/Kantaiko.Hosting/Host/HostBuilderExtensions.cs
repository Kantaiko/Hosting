using Kantaiko.Hosting.Internal;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public static class HostBuilderExtensions
{
    public static void ConfigureKantaikoHosting(this IHostBuilder builder, HostConstructionContext constructionContext,
        IHostLoaderHandler? hostLoaderHandler = null) =>
        builder.ConfigureKantaikoHosting(constructionContext, null, hostLoaderHandler);

    public static void ConfigureKantaikoHosting(this IHostBuilder builder,
        Action<IModuleCollection> moduleConfigureDelegate, IHostLoaderHandler? hostLoaderHandler = null) =>
        builder.ConfigureKantaikoHosting(null, moduleConfigureDelegate, hostLoaderHandler);

    public static void ConfigureKantaikoHosting(this IHostBuilder builder,
        HostConstructionContext? constructionContext = null,
        Action<IModuleCollection>? moduleConfigureDelegate = null,
        IHostLoaderHandler? hostLoaderHandler = null)
    {
        hostLoaderHandler ??= DefaultHostLoaderHandler.Instance;
        constructionContext ??= DefaultHostConstructionContextProvider.Instance.GetHostConstructionContext();

        var moduleCollection = new ModuleCollection();
        moduleCollection.AddModuleTypes(constructionContext.ModuleTypes);
        moduleConfigureDelegate?.Invoke(moduleCollection);

        builder.ConfigureServices((context, services) =>
        {
            services.AddManagedHostServices();

            var hostLoader = new HostLoader(context.Configuration, context.HostingEnvironment);
            var loadedHost = hostLoader.Load(moduleCollection);

            services.AddSingleton(loadedHost.HostInfo);

            foreach (var loadedHostModule in loadedHost.Modules)
            {
                loadedHostModule.Instance.ConfigureServices(services);
            }

            hostLoaderHandler.Handle(loadedHost, services);
        });
    }
}
