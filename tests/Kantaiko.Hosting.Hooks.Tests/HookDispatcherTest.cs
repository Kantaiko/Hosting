using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kantaiko.Hosting.Hooks.Tests;

public class HookDispatcherTest
{
    private class TestHookA : IAsyncHook
    {
        public bool HandlerInvoked { get; set; }
    }

    private class TestHookHandlerA : IAsyncHookHandler<TestHookA>
    {
        public Task HandleAsync(TestHookA payload, CancellationToken cancellationToken)
        {
            payload.HandlerInvoked = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ShouldDispatchAsyncHookToAsyncHandler()
    {
        var hookDispatcher = CreateHookDispatcher();

        var testHookA = new TestHookA();
        await hookDispatcher.DispatchAsync(testHookA);

        Assert.True(testHookA.HandlerInvoked);
    }

    private class TestHookB : IAsyncHook
    {
        public bool HandlerInvoked { get; set; }
    }

    private class TestHookHandlerB : IHookHandler<TestHookB>
    {
        public void Handle(TestHookB payload)
        {
            payload.HandlerInvoked = true;
        }
    }

    [Fact]
    public async Task ShouldDispatchAsyncHookToSyncHandler()
    {
        var hookDispatcher = CreateHookDispatcher();

        var testHookB = new TestHookB();
        await hookDispatcher.DispatchAsync(testHookB);

        Assert.True(testHookB.HandlerInvoked);
    }

    private class TestHookC : IHook
    {
        public bool HandlerInvoked { get; set; }
    }

    private class TestHookHandlerC : IHookHandler<TestHookC>
    {
        public void Handle(TestHookC payload)
        {
            payload.HandlerInvoked = true;
        }
    }

    [Fact]
    public async Task ShouldDispatchSyncHookToSyncHandler()
    {
        var hookDispatcher = CreateHookDispatcher();

        var testHookC = new TestHookC();
        await hookDispatcher.DispatchAsync(testHookC);

        Assert.True(testHookC.HandlerInvoked);
    }

    private static IHookDispatcher CreateHookDispatcher()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddHookServices();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var hookInitializer = serviceProvider.GetRequiredService<HookInitializer>();
        hookInitializer.Initialize(new[] { typeof(HookDispatcherTest).Assembly });

        return serviceProvider.GetRequiredService<IHookDispatcher>();
    }
}
