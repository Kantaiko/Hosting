using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Managed;

public static class SharedServiceProviderExtensions
{
    public static TService GetRequiredService<TService>(this ISharedServiceProvider serviceProvider,
        Func<IServiceProvider, TService> factory) where TService : class
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        return (TService) serviceProvider.GetRequiredService(typeof(TService), factory);
    }

    public static TService GetRequiredService<TService, TImplementation>(this ISharedServiceProvider serviceProvider)
        where TImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        return (TService) serviceProvider.GetRequiredService(typeof(TService),
            sp => ActivatorUtilities.CreateInstance<TImplementation>(sp)!);
    }
}
