using Kantaiko.Hosting.Managed.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Managed;

internal static class ServiceCollectionHelper
{
    public static void AddManagedRuntimeServices(IServiceCollection services)
    {
        services.AddSingleton<RuntimeHostState>();
        services.AddSingleton<IRuntimeHostState>(sp => sp.GetRequiredService<RuntimeHostState>());

        services.AddSingleton<IRuntimeHostManager, RuntimeHostManager>();
    }
}
