using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Hooks;

internal static class ServiceCollectionExtensions
{
    public static void AddHookServices(this IServiceCollection services)
    {
        services.AddTransient<HookInitializer>();
        services.AddTransient<IHookDispatcher, HookDispatcher>();

        services.AddSingleton<HookHandlerCollection>();
        services.AddHostedService<HookHostedService>();
    }
}
