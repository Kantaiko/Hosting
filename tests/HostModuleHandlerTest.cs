using System;
using System.Threading.Tasks;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Loader;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kantaiko.Hosting.Tests
{
    public class HostModuleHandlerTest
    {
        [Fact]
        public async Task ShouldUseProvidedModuleHandlerToProcessLoadedModules()
        {
            var builder = new TestHostBuilder();
            builder.Modules.Add<TestModule>();

            var app = builder.Build();
            await app.StartAsync();

            Assert.Equal(42, builder.TestModuleHandler.MagicNumber);
        }

        private interface ITestModule : IModule
        {
            public int GetValue();
        }

        private class TestModule : ITestModule
        {
            public int GetValue() => 41;
        }

        private class TestHostBuilder : ManagedHostBuilder
        {
            public TestModuleHandler TestModuleHandler { get; } = new();

            public override IManagedHost Build()
            {
                return new ManagedHost(Args, ConstructionContextProvider, TestModuleHandler);
            }
        }

        private class TestModuleHandler : IHostModuleHandler
        {
            public int MagicNumber { get; private set; }

            public void ConfigureServices(IServiceCollection services, LoadedHost host)
            {
                foreach (var loadedModule in host.Modules)
                {
                    if (loadedModule.Instance is not ITestModule instance)
                        continue;

                    MagicNumber = instance.GetValue();
                }
            }

            public void Configure(IServiceProvider provider, LoadedHost host)
            {
                MagicNumber++;
            }
        }
    }
}
