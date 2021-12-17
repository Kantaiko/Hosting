using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity;

public static class ModularHost
{
    public static Task RunAsync<TModule>(string[]? args = null, CancellationToken cancellationToken = default)
        where TModule : IModule
    {
        return Host.CreateDefaultBuilder(args)
            .AddModule<TModule>()
            .CompleteModularityConfiguration()
            .Build().RunAsync(cancellationToken);
    }

    public static void Run<TModule>(string[]? args = null) where TModule : IModule
    {
        RunAsync<TModule>(args).GetAwaiter().GetResult();
    }
}
