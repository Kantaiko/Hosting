using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kantaiko.Hosting.Tests;

public class HostIntegrationTest
{
    [Fact]
    public async Task ShouldCreateStartAndStopManagedHost()
    {
        var builder = new ManagedHostBuilder();

        var app = builder.Build();

        await app.StartAsync();
        Assert.True(app.IsStarted);
        Assert.NotNull(app.Services);

        var waitTask = app.WaitForShutdownAsync();

        await app.StopAsync();

        Assert.True(waitTask.IsCompleted);

        Assert.False(app.IsStarted);
        Assert.Throws<InvalidHostStateException>(() => app.Services);
    }

    [Fact]
    public async Task ShouldCreateAndRestartHost()
    {
        var builder = new ManagedHostBuilder();

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
        var app = new ManagedHost();

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
        var app = new ManagedHost();

        _ = app.StopAsync();

        async Task Action()
        {
            await app.StopAsync();
        }

        await Assert.ThrowsAsync<InvalidHostStateException>(Action);
    }

    [Fact]
    public async Task ShouldReportRestartFailureAndRestorePreviousHost()
    {
        var typeProvider = new TestConstructionContextProvider();

        var app = new ManagedHost(typeProvider);
        await app.StartAsync();

        typeProvider.ProvideFailingModule = true;

        var hostState = await app.RestartAsync();
        Assert.True(hostState.RestartFailed);
        Assert.IsType<TestException>(hostState.StartupException);

        await app.StopAsync();
    }

    private class TestException : Exception { }

    private class TestFailingModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            throw new TestException();
        }
    }

    private class TestConstructionContextProvider : IHostConstructionContextProvider
    {
        public bool ProvideFailingModule { get; set; }

        public HostConstructionContext GetHostConstructionContext()
        {
            return ProvideFailingModule
                ? new HostConstructionContext(moduleTypes: new[] { typeof(TestFailingModule) })
                : new HostConstructionContext();
        }
    }
}
