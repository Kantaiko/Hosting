using Kantaiko.Hosting.Managed.Exceptions;
using Xunit;

namespace Kantaiko.Hosting.Managed.Tests;

public class ManagedHostIntegrationTest
{
    [Fact]
    public async Task ShouldCreateStartAndStopManagedHost()
    {
        var builder = ManagedHost.CreateDefaultBuilder();

        var app = builder.Build();
        await app.StartAsync();

        Assert.True(app.IsStarted);
        Assert.NotNull(app.Services);

        var waitTask = app.WaitForShutdownAsync();

        await app.StopAsync();

        await Task.Yield();
        Assert.True(waitTask.IsCompleted);

        Assert.False(app.IsStarted);
        Assert.Throws<InvalidHostStateException>(() => app.Services);
    }

    [Fact]
    public async Task ShouldCreateAndRestartHost()
    {
        var builder = ManagedHost.CreateDefaultBuilder();

        var app = builder.Build();
        await app.StartAsync();

        var restartTask = app.RestartAsync();
        Assert.Equal(ManagedHostState.Restarting, app.State);

        var runtimeState = await restartTask;
        Assert.True(runtimeState.HostRestarted);
        Assert.True(app.IsStarted);
    }

    [Fact]
    public async Task ShouldReportDoubleStart()
    {
        var builder = ManagedHost.CreateDefaultBuilder();
        var app = builder.Build();

        _ = app.StartAsync();

        async Task Action()
        {
            await app.StartAsync();
        }

        await Assert.ThrowsAsync<InvalidHostStateException>(Action);

        await app.StopAsync();
    }

    [Fact]
    public async Task ShouldReportDoubleStop()
    {
        var builder = ManagedHost.CreateDefaultBuilder();
        var app = builder.Build();

        await app.StartAsync();

        _ = app.StopAsync();

        async Task Action()
        {
            await app.StopAsync();
        }

        await Assert.ThrowsAsync<InvalidHostStateException>(Action);
    }
}
