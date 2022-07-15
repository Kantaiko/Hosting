using Kantaiko.Hosting.Lifecycle.TypeRegistration;
using Kantaiko.Hosting.Managed;
using Kantaiko.Hosting.Modularity.TypeRegistration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Lifecycle;

public static class ManagedHostBuilderExtensions
{
    public static void UseManagedHostLifecycle(this IManagedHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);

        var hostLifecycle = new ManagedHostLifecycle();

        hostBuilder.UseManagedHostHandler(hostLifecycle);

        hostBuilder.ConfigureHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddTypeRegistrationHandler<ManagedHostLifecycleRegistrationHandler>();

                services.AddSingleton(hostLifecycle);
                services.AddSingleton<IManagedHostLifecycle>(sp => sp.GetRequiredService<ManagedHostLifecycle>());
            });
        });
    }
}
