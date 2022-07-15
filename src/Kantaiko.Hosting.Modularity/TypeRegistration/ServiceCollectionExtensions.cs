using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modularity.TypeRegistration;

public static class ServiceCollectionExtensions
{
    public static void AddTypeRegistrationHandler<THandler>(this IServiceCollection services)
        where THandler : class, ITypeRegistrationHandler
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<TypeRegistrationManager>();
        services.AddTransient<ITypeRegistrationHandler, THandler>();
    }
}
