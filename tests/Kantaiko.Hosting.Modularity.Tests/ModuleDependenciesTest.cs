using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kantaiko.Hosting.Modularity.Tests;

public class ModuleDependenciesTest
{
    private class ModuleA : Module
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<object>(42);
        }
    }

    private class ModuleB : Module
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddModule<ModuleA>();
        }
    }

    [Fact]
    public void ShouldInitializeDependencyModules()
    {
        var hostBuilder = new HostBuilder();

        hostBuilder.ConfigureServices(services => services.AddModule<ModuleB>());

        var host = hostBuilder.Build();

        Assert.Equal(42, host.Services.GetRequiredService<object>());
    }
}
