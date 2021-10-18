using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public static class ManagedHostBuilderExtensions
{
    public static void ConfigureServices(this IManagedHostBuilder builder,
        Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        builder.ConfigureHostBuilder(hostBuilder => hostBuilder.ConfigureServices(configureDelegate));
    }

    public static void ConfigureServices(this IManagedHostBuilder builder,
        Action<IServiceCollection> configureDelegate)
    {
        builder.ConfigureHostBuilder(hostBuilder => hostBuilder.ConfigureServices(configureDelegate));
    }
}
