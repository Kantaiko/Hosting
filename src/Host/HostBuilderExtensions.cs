using Kantaiko.Hosting.Internal;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host
{
    public static class HostBuilderExtensions
    {
        public static void ConfigureKantaikoHosting(this IHostBuilder builder,
            HostConstructionContext constructionContext,
            IHostModuleHandler? hostModuleHandler = null)
        {
            var moduleCollection = new ModuleCollection();
            moduleCollection.AddModuleTypes(constructionContext.ModuleTypes);

            builder.UseServiceProviderFactory(context =>
            {
                var hostLoader = new HostLoader(context.Configuration, context.HostingEnvironment);
                var loadedHost = hostLoader.Load(moduleCollection);

                return new ServiceProviderFactory(loadedHost, hostModuleHandler);
            });

            builder.ConfigureServices(services => services.AddManagedHostServices());
        }
    }
}
