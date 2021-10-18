using Kantaiko.Hosting.Hooks.HostHooks;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kantaiko.Hosting.Hooks.Tests;

public class ManagedHostLifecycleHookTest
{
    private class TestHostState
    {
        public int MagicNumber { get; set; }
    }

    private class TestModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<TestHostState>();
        }
    }


    private class TestHostRestartHandler : IHookHandler<ManagedHostRestartedHook>
    {
        private readonly TestHostState _testHostState;

        public TestHostRestartHandler(TestHostState testHostState)
        {
            _testHostState = testHostState;
        }

        public void Handle(ManagedHostRestartedHook payload)
        {
            _testHostState.MagicNumber = 42;
        }
    }

    private class TestHostRestartFailHandler : IHookHandler<ManagedHostRestartFailedHook>
    {
        private readonly TestHostState _testHostState;

        public TestHostRestartFailHandler(TestHostState testHostState)
        {
            _testHostState = testHostState;
        }

        public void Handle(ManagedHostRestartFailedHook payload)
        {
            _testHostState.MagicNumber = 24;
        }
    }

    private class TestFailingModule : IModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            throw new Exception();
        }
    }

    [Fact]
    public async Task ShouldDispatchHookOnHostRestart()
    {
        var builder = new ManagedHostBuilder();
        builder.ConfigureHooks();

        builder.Modules.Add<TestModule>();

        using var app = builder.Build();
        await app.StartAsync();

        await app.RestartAsync();

        var testHostState = app.Services.GetRequiredService<TestHostState>();
        Assert.Equal(42, testHostState.MagicNumber);

        await app.StopAsync();
    }
}
