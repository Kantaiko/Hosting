using Kantaiko.Hosting.Modularity.Introspection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VerifyXunit;
using Xunit;

namespace Kantaiko.Hosting.Modularity.Tests;

[UsesVerify]
public class IntrospectionTest
{
    [Module(DisplayName = "CustomTitle")]
    private class ModuleC : Module { }

    private class TestModule : Module
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddModule<ModuleC>();
        }
    }

    private class ModuleA : Module { }

    private class ModuleB : Module
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddModule<ModuleA>();
            services.AddModule<TestModule>();
        }
    }

    [Fact]
    public async Task ShouldContainValidIntrospectionInfo()
    {
        var hostBuilder = new HostBuilder();

        hostBuilder.AddModule<ModuleB>();

        var host = hostBuilder.Build();
        var hostInfo = host.Services.GetRequiredService<HostInfo>();

        await Verify(hostInfo)
            .IgnoreMembers("Version", "Assemblies")
            .UseDirectory("__snapshots__");
    }
}
