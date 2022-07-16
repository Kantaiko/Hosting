using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kantaiko.Hosting.Modularity;
using Xunit;

namespace Kantaiko.Hosting.Lifecycle.Tests;

public class ApplicationLifecycleTest
{
    [Fact]
    public async Task ShouldDispatchLifecycleEvents()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<TestState>();
            services.AddModule<TestModule>();
            services.AddApplicationLifecycle();
        });

        var host = builder.Build();

        var lifecycle = host.Services.GetRequiredService<IApplicationLifecycle>();

        var state = 0;

        lifecycle.ApplicationStarting += _ =>
        {
            Assert.Equal(0, state++);
            return Task.CompletedTask;
        };

        lifecycle.ApplicationStarted += _ =>
        {
            //
            Assert.Equal(1, state++);
        };

        lifecycle.ApplicationStopping += _ =>
        {
            Assert.Equal(2, state++);
            return Task.CompletedTask;
        };

        lifecycle.ApplicationStopped += _ =>
        {
            //
            Assert.Equal(3, state++);
        };

        await host.StartAsync();
        await host.StopAsync();

        Assert.Equal(4, state);
    }

    private class TestModule : Module { }
}
