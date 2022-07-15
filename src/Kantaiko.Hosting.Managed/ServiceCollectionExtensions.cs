using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Managed;

public static class ServiceCollectionExtensions
{
    public static void AddSharedService<TService>(this IServiceCollection services) where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(sp => sp.GetRequiredService<ISharedServiceProvider>().GetRequiredService<TService>());
    }

    public static void AddSharedService<TService>(this IServiceCollection services,
        Func<IServiceProvider, TService> factory) where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(sp => sp.GetRequiredService<ISharedServiceProvider>().GetRequiredService(factory));
    }

    public static void AddSharedService<TService, TImplementation>(this IServiceCollection services)
        where TService : class where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(sp =>
            sp.GetRequiredService<ISharedServiceProvider>().GetRequiredService<TService, TImplementation>());
    }
}
