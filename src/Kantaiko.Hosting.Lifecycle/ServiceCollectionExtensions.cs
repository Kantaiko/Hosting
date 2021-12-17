using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Lifecycle;

public static class ServiceCollectionExtensions
{
    public static void AddLifecycleEvents(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();

        services.AddSingleton<IApplicationLifecycle, ApplicationLifecycle>();
    }

    public static void AddLifecycleEvents(this IServiceCollection services, IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();

        services.AddSingleton<IApplicationLifecycle>(_ => new ApplicationLifecycle(types));
    }

    public static void AddLifecycleEvents(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();

        var types = assemblies.SelectMany(x => x.GetTypes()).ToImmutableArray();
        services.AddSingleton<IApplicationLifecycle>(_ => new ApplicationLifecycle(types));
    }

    public static void AddLifecycleEvents(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();

        services.AddSingleton<IApplicationLifecycle>(_ => new ApplicationLifecycle(assembly.GetTypes()));
    }
}
