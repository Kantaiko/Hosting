using System.Collections.Immutable;
using System.Reflection;
using Kantaiko.Routing.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Lifecycle;

public static class ServiceCollectionExtensions
{
    public static void AddLifecycleEvents(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();
        services.AddRoutingContextAccessor();

        services.AddSingleton<IApplicationLifecycle, ApplicationLifecycle>();
    }

    public static void AddLifecycleEvents(this IServiceCollection services, IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();
        services.AddRoutingContextAccessor();

        services.AddSingleton<IApplicationLifecycle>(_ => new ApplicationLifecycle(types));
    }

    public static void AddLifecycleEvents(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();
        services.AddRoutingContextAccessor();

        var types = assemblies.SelectMany(x => x.GetTypes()).ToImmutableArray();
        services.AddSingleton<IApplicationLifecycle>(_ => new ApplicationLifecycle(types));
    }

    public static void AddLifecycleEvents(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<LifecycleHostedService>();
        services.AddRoutingContextAccessor();

        services.AddSingleton<IApplicationLifecycle>(_ => new ApplicationLifecycle(assembly.GetTypes()));
    }

    public static void AddRoutingContextAccessor(this IServiceCollection services)
    {
        services.AddScoped<ContextAccessor>();
        services.AddTransient<IContextAcceptor>(sp => sp.GetRequiredService<ContextAccessor>());
        services.AddTransient<IContextAccessor>(sp => sp.GetRequiredService<ContextAccessor>());
        services.AddTransient(typeof(IContextAccessor<>), typeof(ContextAccessor<>));
    }
}
