using Kantaiko.Hosting.Lifecycle;
using Kantaiko.Hosting.Managed;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting;

public static class ModularManagedHost
{
    public static IManagedHostBuilder CreateDefaultBuilder(string[]? args = null)
    {
        return ManagedHost.CreateDefaultBuilder(args)
            .ConfigureHostBuilder(ConfigureHost)
            .UseManagedHostHandler(new LifecycleManagedHostHandler())
            .UseHostBuilderFactory(new ModularHostBuilderFactory(args));
    }

    private static void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(ServiceCollectionExtensions.AddModularLifecycleEvents);
        hostBuilder.ConfigureServices(ServiceCollectionExtensions.AddManagedHostLifecycle);
    }
}
