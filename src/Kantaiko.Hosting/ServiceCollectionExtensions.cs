using System.Collections.Immutable;
using Kantaiko.Hosting.Lifecycle;
using Kantaiko.Hosting.Modularity.Introspection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kantaiko.Hosting;

public static class ServiceCollectionExtensions
{
    public static void AddModularLifecycleEvents(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLifecycleEvents();

        services.Replace(ServiceDescriptor.Singleton<IApplicationLifecycle>(sp =>
        {
            var hostInfo = sp.GetRequiredService<HostInfo>();
            var types = hostInfo.Assemblies.SelectMany(x => x.GetTypes()).ToImmutableArray();

            return new ApplicationLifecycle(types);
        }));
    }
}
