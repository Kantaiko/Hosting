using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Hosting.Managed.Runtime;
using Kantaiko.Hosting.Modularity;
using Kantaiko.Routing;
using Kantaiko.Routing.Events;
using Xunit;

namespace Kantaiko.Hosting.Tests;

public class ModularManagedHostTest
{
    [Fact]
    public async Task ShouldStartRestartAndStopHost()
    {
        await ModularManagedHost.RunAsync<TestModule>();
    }

    private class TestModule : Module { }

    private class HostInitiallyStartedHandler : Routing.Events.EventHandler<HostInitiallyStartedEvent>
    {
        private readonly IRuntimeHostManager _runtimeHostManager;

        public HostInitiallyStartedHandler(IRuntimeHostManager runtimeHostManager)
        {
            _runtimeHostManager = runtimeHostManager;
        }

        protected override Task<Unit> HandleAsync(IEventContext<HostInitiallyStartedEvent> context)
        {
            _runtimeHostManager.Restart();

            return Unit.Task;
        }
    }

    private class HostTransitionCompletedHandler : Routing.Events.EventHandler<HostTransitionCompletedEvent>
    {
        private readonly IRuntimeHostManager _runtimeHostManager;

        public HostTransitionCompletedHandler(IRuntimeHostManager runtimeHostManager)
        {
            _runtimeHostManager = runtimeHostManager;
        }

        protected override Task<Unit> HandleAsync(IEventContext<HostTransitionCompletedEvent> context)
        {
            var hostState = context.Event.HostState;

            Assert.True(hostState.HostRestarted);
            _runtimeHostManager.Stop();

            return Unit.Task;
        }
    }
}
