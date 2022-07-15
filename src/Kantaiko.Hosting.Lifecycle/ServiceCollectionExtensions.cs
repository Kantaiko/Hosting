using Kantaiko.Hosting.Lifecycle.TypeRegistration;
using Kantaiko.Hosting.Modularity.TypeRegistration;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Lifecycle;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationLifecycle(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTypeRegistrationHandler<ApplicationLifecycleRegistrationHandler>();

        services.AddSingleton<ApplicationLifecycle>();
        services.AddHostedService(sp => sp.GetRequiredService<ApplicationLifecycle>());
        services.AddSingleton<IApplicationLifecycle>(sp => sp.GetRequiredService<ApplicationLifecycle>());
    }
}
