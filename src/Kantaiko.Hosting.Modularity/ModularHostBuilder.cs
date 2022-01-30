using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity;

public class ModularHostBuilder : IHostBuilder
{
    private readonly IHostBuilder _hostBuilder;

    public IDictionary<object, object> Properties => _hostBuilder.Properties;

    public ModularHostBuilder(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder;
    }

    public ModularHostBuilder()
    {
        _hostBuilder = new HostBuilder();
    }

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        _hostBuilder.ConfigureHostConfiguration(configureDelegate);
        return this;
    }

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _hostBuilder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        _hostBuilder.ConfigureServices(configureDelegate);
        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        where TContainerBuilder : notnull
    {
        _hostBuilder.UseServiceProviderFactory(factory);
        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(
        Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
    {
        _hostBuilder.UseServiceProviderFactory(factory);
        return this;
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(
        Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _hostBuilder.ConfigureContainer(configureDelegate);
        return this;
    }

    public IHost Build()
    {
        _hostBuilder.CompleteModularityConfiguration();
        return _hostBuilder.Build();
    }
}
