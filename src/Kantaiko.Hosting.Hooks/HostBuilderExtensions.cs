using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Hooks;

public static class HostBuilderExtensions
{
    public static void ConfigureKantaikoHostingHooks(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services => services.AddHookServices());
    }
}
