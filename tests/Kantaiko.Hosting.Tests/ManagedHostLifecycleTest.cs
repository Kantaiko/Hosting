using Kantaiko.Hosting.Lifecycle;
using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Hosting.Managed;
using Kantaiko.Hosting.Managed.Runtime;
using Kantaiko.Hosting.Modularity;
using Kantaiko.Routing.Events;
using Xunit;

namespace Kantaiko.Hosting.Tests;

public class ManagedHostLifecycleTest
{
    [Fact]
    public async Task ShouldStartRestartAndStopHost()
    {
        var builder = ManagedHost.CreateDefaultBuilder();

        builder.UseManagedHostLifecycle();

        builder.ConfigureHostBuilder(b => b.AddModule<TestModule>());

        var host = builder.Build();

        await host.RunAsync();
    }

    private class TestModule : Module { }

    private class HostInitiallyStartedHandler : AsyncEventHandlerBase<HostInitiallyStartedEvent>
    {
        private readonly IRuntimeHostManager _runtimeHostManager;

        public HostInitiallyStartedHandler(IRuntimeHostManager runtimeHostManager)
        {
            _runtimeHostManager = runtimeHostManager;
        }

        protected override Task HandleAsync(IAsyncEventContext<HostInitiallyStartedEvent> context)
        {
            _runtimeHostManager.Restart();

            return Task.CompletedTask;
        }
    }

    private class HostTransitionCompletedHandler : AsyncEventHandlerBase<HostTransitionCompletedEvent>
    {
        private readonly IRuntimeHostManager _runtimeHostManager;

        public HostTransitionCompletedHandler(IRuntimeHostManager runtimeHostManager)
        {
            _runtimeHostManager = runtimeHostManager;
        }

        protected override Task HandleAsync(IAsyncEventContext<HostTransitionCompletedEvent> context)
        {
            var hostState = context.Event.HostState;

            Assert.True(hostState.HostRestarted);
            _runtimeHostManager.Stop();

            return Task.CompletedTask;
        }
    }
}
