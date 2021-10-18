using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Modules;
using Xunit;

namespace Kantaiko.Hosting.Tests;

public class SingleHostTest
{
    internal class TestModule : IModule { }

    [Fact]
    public async Task ShouldCreateStartAndStopSingleHost()
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder();
        builder.ConfigureKantaikoHosting(modules => modules.Add<TestModule>());

        using var host = builder.Build();
        await host.StartAsync();
        await host.StopAsync();
    }
}
