using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

public class DefaultHostBuilderFactory : IHostBuilderFactory
{
    private readonly string[]? _args;

    public DefaultHostBuilderFactory(string[]? args = null)
    {
        _args = args;
    }

    public IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder(_args);
    }
}
