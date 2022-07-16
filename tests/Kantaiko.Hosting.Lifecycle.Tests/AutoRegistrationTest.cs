using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Hosting.Modularity;
using Kantaiko.Routing.Events;
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
            services.AddModule<TestModule>();
            services.AddApplicationLifecycle();
        });

        var host = builder.Build();

        await host.StartAsync();
        await host.StopAsync();

        var state = host.Services.GetRequiredService<TestState>();
        Assert.Equal(4, state.Count);
    }

    private class TestModule : Module { }


    private class ApplicationStartingHandler : AsyncEventHandlerBase<ApplicationStartingEvent>
    {
        private readonly TestState _testState;

        public ApplicationStartingHandler(TestState testState)
        {
            _testState = testState;
        }

        protected override Task HandleAsync(IAsyncEventContext<ApplicationStartingEvent> context)
        {
            Assert.Equal(0, _testState.Count++);

            return Task.CompletedTask;
        }
    }

    private class ApplicationStartedHandler : EventHandlerBase<ApplicationStartedEvent>
    {
        private readonly TestState _testState;

        public ApplicationStartedHandler(TestState testState)
        {
            _testState = testState;
        }

        protected override void Handle(IEventContext<ApplicationStartedEvent> context)
        {
            Assert.Equal(1, _testState.Count++);
        }
    }

    private class ApplicationStoppingHandler : AsyncEventHandlerBase<ApplicationStoppingEvent>
    {
        private readonly TestState _testState;

        public ApplicationStoppingHandler(TestState testState)
        {
            _testState = testState;
        }

        protected override Task HandleAsync(IAsyncEventContext<ApplicationStoppingEvent> context)
        {
            Assert.Equal(2, _testState.Count++);

            return Task.CompletedTask;
        }
    }

    private class ApplicationStoppedHandler : EventHandlerBase<ApplicationStoppedEvent>
    {
        private readonly TestState _testState;

        public ApplicationStoppedHandler(TestState testState)
        {
            _testState = testState;
        }

        protected override void Handle(IEventContext<ApplicationStoppedEvent> context)
        {
            Assert.Equal(3, _testState.Count++);
        }
    }
}
