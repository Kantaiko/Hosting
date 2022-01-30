using Kantaiko.Hosting.Managed;
using Kantaiko.Hosting.Modularity;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting;

public class ModularHostBuilderFactory : IHostBuilderFactory
{
    private readonly string[]? _args;

    public ModularHostBuilderFactory(string[]? args = null)
    {
        _args = args;
    }

    public IHostBuilder CreateHostBuilder()
    {
        return new ModularHostBuilder(Host.CreateDefaultBuilder(_args));
    }
}
