using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Hooks;

internal static class ServiceCollectionExtensions
{
    public static void AddHookServices(this IServiceCollection services)
    {
        services.AddSingleton<HookHandlerCollection>();
        services.AddTransient<HookInitializer>();
        services.AddScoped<IHookDispatcher, HookDispatcher>();
        services.AddHostedService<HookHostedService>();
    }
}
