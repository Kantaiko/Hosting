using Kantaiko.Hosting.Host;

namespace Kantaiko.Hosting.Hooks;

public static class ManagedHostBuilderExtensions
{
    public static void ConfigureHooks(this IManagedHostBuilder builder)
    {
        builder.UseHostLifecycleHandler(HookHostLifecycleHandler.Instance);
        builder.ConfigureHostBuilder(hostBuilder => hostBuilder.ConfigureKantaikoHostingHooks());
    }
}
