using System.Reflection;
using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kantaiko.Hosting.Lifecycle.Tests;

public class AutoRegistrationTest
{
    [Fact]
    public async Task ShouldUseClassHandlers()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<TestState>();
            services.AddLifecycleEvents(Assembly.GetExecutingAssembly());
        });

        var host = builder.Build();

        await host.StartAsync();
        await host.StopAsync();

        var state = host.Services.GetRequiredService<TestState>();
        Assert.Equal(4, state.Count);
    }

    private class TestState
    {
        public int Count { get; set; }
    }

    private class ApplicationStartingHandler : LifecycleEventHandler<ApplicationStartingEvent>
    {
        private readonly TestState _testState;

        public ApplicationStartingHandler(TestState testState)
        {
            _testState = testState;
        }

        public override Task<Unit> Handle(LifecycleEventContext<ApplicationStartingEvent> context)
        {
            Assert.Equal(0, _testState.Count++);
            return Unit.Task;
        }
    }

    private class ApplicationStartedHandler : LifecycleEventHandler<ApplicationStartedEvent>
    {
        private readonly TestState _testState;

        public ApplicationStartedHandler(TestState testState)
        {
            _testState = testState;
        }

        public override Task<Unit> Handle(LifecycleEventContext<ApplicationStartedEvent> context)
        {
            Assert.Equal(1, _testState.Count++);
            return Unit.Task;
        }
    }

    private class ApplicationStoppingHandler : LifecycleEventHandler<ApplicationStoppingEvent>
    {
        private readonly TestState _testState;

        public ApplicationStoppingHandler(TestState testState)
        {
            _testState = testState;
        }

        public override Task<Unit> Handle(LifecycleEventContext<ApplicationStoppingEvent> context)
        {
            Assert.Equal(2, _testState.Count++);
            return Unit.Task;
        }
    }

    private class ApplicationStoppedHandler : LifecycleEventHandler<ApplicationStoppedEvent>
    {
        private readonly TestState _testState;

        public ApplicationStoppedHandler(TestState testState)
        {
            _testState = testState;
        }

        public override Task<Unit> Handle(LifecycleEventContext<ApplicationStoppedEvent> context)
        {
            Assert.Equal(3, _testState.Count++);
            return Unit.Task;
        }
    }
}
