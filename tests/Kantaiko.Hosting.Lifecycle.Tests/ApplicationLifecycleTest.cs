using Kantaiko.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kantaiko.Hosting.Lifecycle.Tests;

public class ApplicationLifecycleTest
{
    [Fact]
    public async Task ShouldDispatchLifecycleEvents()
    {
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureServices(services => services.AddLifecycleEvents());

        var host = builder.Build();

        var lifecycle = host.Services.GetRequiredService<IApplicationLifecycle>();

        var state = 0;

        lifecycle.ApplicationStarting = lifecycle.ApplicationStarting.Wrap((context, next) =>
        {
            Assert.Equal(0, state++);
            return next(context);
        });

        lifecycle.ApplicationStarted = lifecycle.ApplicationStarted.Wrap((context, next) =>
        {
            Assert.Equal(1, state++);
            return next(context);
        });

        lifecycle.ApplicationStopping = lifecycle.ApplicationStopping.Wrap((context, next) =>
        {
            Assert.Equal(2, state++);
            return next(context);
        });

        lifecycle.ApplicationStopped = lifecycle.ApplicationStopped.Wrap((context, next) =>
        {
            Assert.Equal(3, state++);
            return next(context);
        });

        await host.StartAsync();
        await host.StopAsync();

        Assert.Equal(4, state);
    }
}
