using Kantaiko.Hosting.Managed.Exceptions;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kantaiko.Hosting.Managed.Tests;

public class FailureRecoveryTest
{
    [Fact]
    public async Task ShouldRecoverRestartFailureUsingPreviousHostBuilderFactory()
    {
        var hostBuilderFactoryProvider = new TestHostBuilderFactoryProvider();
        var managedHost = new ManagedHost(hostBuilderFactoryProvider);

        await managedHost.StartAsync();

        hostBuilderFactoryProvider.TriggerFailureForNewBuilders = true;

        var state = await managedHost.RestartAsync();

        await managedHost.StopAsync();

        Assert.True(state.RestartFailed);
        Assert.IsType<TestException>(state.StartupException);
    }

    [Fact]
    public async Task ShouldReportUnrecoverableFailureToAllListeners()
    {
        var hostBuilderFactoryProvider = new TestHostBuilderFactoryProvider();
        var managedHost = new ManagedHost(hostBuilderFactoryProvider);

        await managedHost.StartAsync();

        hostBuilderFactoryProvider.TriggerFailureForAllBuilders = true;

        var shutdownTask = managedHost.WaitForShutdownAsync();

        await Assert.ThrowsAsync<UnrecoverableHostStateException>(() => managedHost.RestartAsync());

        Assert.True(shutdownTask.IsFaulted);
        Assert.IsType<AggregateException>(shutdownTask.Exception);
        Assert.IsType<UnrecoverableHostStateException>(shutdownTask.Exception!.InnerException);
    }

    private class TestHostBuilderFactoryProvider : IHostBuilderFactoryProvider
    {
        public bool Volatile => true;

        public bool TriggerFailureForNewBuilders { get; set; }
        public bool TriggerFailureForAllBuilders { get; set; }

        public Task<IHostBuilderFactory> GetHostBuilderFactoryAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IHostBuilderFactory>(new TestHostBuilderFactory(this, TriggerFailureForNewBuilders));
        }
    }

    private class TestHostBuilderFactory : IHostBuilderFactory
    {
        private readonly TestHostBuilderFactoryProvider _provider;
        private readonly bool _triggerFailure;

        public TestHostBuilderFactory(TestHostBuilderFactoryProvider provider, bool triggerFailure)
        {
            _provider = provider;
            _triggerFailure = triggerFailure;
        }

        public IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = new HostBuilder();

            if (_triggerFailure || _provider.TriggerFailureForAllBuilders)
            {
                hostBuilder.ConfigureServices(_ => throw new TestException());
            }

            return hostBuilder;
        }
    }

    private class TestException : Exception { }
}
