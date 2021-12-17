using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kantaiko.Hosting.Modularity.Tests;

public class CommonModularityTest
{
    private class TestModule : Module
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            Assert.DoesNotContain(services, x => x.ServiceType == typeof(object));

            services.AddSingleton<object>(42);
        }
    }

    [Fact]
    public void ShouldAddModule()
    {
        var hostBuilder = new HostBuilder();
        hostBuilder.AddModule<TestModule>();

        var host = hostBuilder.Build();
        Assert.Equal(42, host.Services.GetRequiredService<object>());
    }

    [Fact]
    public void ShouldInitializeModuleOnce()
    {
        var hostBuilder = new HostBuilder();

        hostBuilder.ConfigureServices(services =>
        {
            services.AddModule<TestModule>();
            services.AddModule<TestModule>();
        });

        var host = hostBuilder.Build();
        Assert.Equal(42, host.Services.GetRequiredService<object>());
    }

    [Fact]
    public void ShouldCheckModuleRegistration()
    {
        var hostBuilder = new HostBuilder();

        hostBuilder.ConfigureServices(services =>
        {
            Assert.False(services.IsModuleRegistered<TestModule>());

            services.AddModule<TestModule>();

            Assert.True(services.IsModuleRegistered<TestModule>());
        });

        var host = hostBuilder.Build();
        Assert.Equal(42, host.Services.GetRequiredService<object>());
    }

    [Fact]
    public void ShouldAddModuleByRuntimeType()
    {
        var hostBuilder = new HostBuilder();
        hostBuilder.AddModule(typeof(TestModule));

        var host = hostBuilder.Build();
        Assert.Equal(42, host.Services.GetRequiredService<object>());
    }
}
