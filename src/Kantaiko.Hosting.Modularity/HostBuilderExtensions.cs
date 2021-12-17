using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddModule<TModule>(this IHostBuilder hostBuilder) where TModule : IModule
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.ConfigureServices(services => services.AddModule<TModule>());
    }

    public static IHostBuilder AddModule(this IHostBuilder hostBuilder, Type moduleType)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.ConfigureServices(services => services.AddModule(moduleType));
    }

    public static IHostBuilder CompleteModularityConfiguration(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        return hostBuilder.ConfigureServices(services => services.CompleteModularityConfiguration());
    }
}
