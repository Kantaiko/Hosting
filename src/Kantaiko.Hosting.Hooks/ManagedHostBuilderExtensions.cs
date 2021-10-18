using Kantaiko.Hosting.Host;

namespace Kantaiko.Hosting.Hooks;

public static class ManagedHostBuilderExtensions
{
    public static void ConfigureHooks(this IManagedHostBuilder builder)
    {
        builder.UseHostLoaderHandler(HookHostLoaderHandler.Instance);
        builder.UseHostLifecycleHandler(HookHostLifecycleHandler.Instance);
    }
}
