using Kantaiko.Hosting.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Internal;

internal static class ServiceCollectionExtensions
{
    public static void AddManagedHostServices(this IServiceCollection services)
    {
        services.AddRuntimeServices();
    }

    private static void AddRuntimeServices(this IServiceCollection services)
    {
        services.AddSingleton<RuntimeHostState>();
        services.AddSingleton<IRuntimeHostState>(sp => sp.GetRequiredService<RuntimeHostState>());

        services.AddSingleton<RuntimeHostManager>();
        services.AddSingleton<IRuntimeHostManager>(sp => sp.GetRequiredService<RuntimeHostManager>());
    }
}
