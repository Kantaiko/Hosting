using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Hooks;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Internal;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kantaiko.Hosting.Tests
{
    public class HostIntegrationTest
    {
        [Fact]
        public async Task ShouldCreateStartAndStopManagedHost()
        {
            var builder = new ManagedHostBuilder();
            builder.Modules.Add<TestModule>();

            var app = builder.Build();

            await app.StartAsync();
            Assert.True(app.IsStarted);
            Assert.NotNull(app.Services);

            var testState = app.Services!.GetRequiredService<TestState>();
            Assert.True(testState.IsStarted);

            await app.StopAsync();

            Assert.False(app.IsStarted);
            Assert.Null(app.Services);
            Assert.False(testState.IsStarted);
        }

        [Fact]
        public async Task ShouldCreateAndRestartHost()
        {
            var builder = new ManagedHostBuilder();
            builder.Modules.Add<TestModule>();

            var app = builder.Build();
            await app.StartAsync();

            Debug.Assert(app.Services is not null);

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

            var app = new ManagedHost(hostConstructionContextProvider: typeProvider);
            await app.StartAsync();

            typeProvider.ProvideFailingModule = true;

            var hostState = await app.RestartAsync();
            Assert.True(hostState.RestartFailed);
            Assert.IsType<TestException>(hostState.StartupException);

            await app.StopAsync();
        }

        private class TestState
        {
            public bool IsStarted { get; set; }
        }

        private class TestApplicationStartupHandler : IHookHandler<ApplicationStartupHook>
        {
            private readonly TestState? _testState;

            public TestApplicationStartupHandler(IServiceProvider serviceProvider)
            {
                _testState = serviceProvider.GetService<TestState>();
            }

            public void Handle(ApplicationStartupHook payload)
            {
                if (_testState is not null)
                    _testState.IsStarted = true;
            }
        }

        private class TestApplicationShutdownHandler : IHookHandler<ApplicationShutdownHook>
        {
            private readonly TestState? _testState;

            public TestApplicationShutdownHandler(IServiceProvider serviceProvider)
            {
                _testState = serviceProvider.GetService<TestState>();
            }

            public void Handle(ApplicationShutdownHook payload)
            {
                if (_testState is not null)
                    _testState.IsStarted = false;
            }
        }

        private class TestModule : IModule
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton<TestState>();
                services.AddRuntimeServices();
            }
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
                    ? new HostConstructionContext(new[] {typeof(TestFailingModule)})
                    : new HostConstructionContext();
            }
        }
    }
}
