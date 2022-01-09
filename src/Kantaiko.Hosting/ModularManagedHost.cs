using Kantaiko.Hosting.Lifecycle;
using Kantaiko.Hosting.Managed;
using Kantaiko.Hosting.Modularity;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting;

public static class ModularManagedHost
{
    public static Task RunAsync<TModule>(string[]? args = null, CancellationToken cancellationToken = default)
        where TModule : IModule
    {
        static void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder.AddModule<TModule>();
            hostBuilder.CompleteModularityConfiguration();
            hostBuilder.ConfigureServices(ServiceCollectionExtensions.AddModularLifecycleEvents);
            hostBuilder.ConfigureServices(ServiceCollectionExtensions.AddManagedHostLifecycle);
        }

        return ManagedHost.CreateDefaultBuilder(args)
            .UseManagedHostHandler(new LifecycleManagedHostHandler())
            .ConfigureHostBuilder(ConfigureHost)
            .Build().RunAsync(cancellationToken);
    }

    public static void Run<TModule>(string[]? args = null)
        where TModule : IModule
    {
        RunAsync<TModule>(args).GetAwaiter().GetResult();
    }
}
