using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

public class DefaultHostBuilderFactory : IHostBuilderFactory
{
    private readonly string[]? _args;
    private readonly ISharedServiceProvider _sharedServiceProvider;

    public DefaultHostBuilderFactory(string[]? args = null, ISharedServiceProvider? sharedServiceProvider = null)
    {
        _args = args;
        _sharedServiceProvider = sharedServiceProvider ?? new SharedServiceProvider();
    }

    public IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder(_args);

        builder.ConfigureServices(services => services.AddSingleton(_sharedServiceProvider));

        return builder;
    }
}
