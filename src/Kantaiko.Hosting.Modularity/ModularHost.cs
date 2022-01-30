using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity;

public static class ModularHost
{
    public static IHostBuilder CreateDefaultBuilder(string[]? args = null)
    {
        return new ModularHostBuilder(Host.CreateDefaultBuilder(args));
    }
}
