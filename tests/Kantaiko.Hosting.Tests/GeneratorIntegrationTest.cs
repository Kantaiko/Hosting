using Kantaiko.Hosting.Modularity;
using Kantaiko.Hosting.Modularity.Configuration;
using Kantaiko.Hosting.Modularity.Generator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Kantaiko.Hosting.Tests;

public class GeneratorIntegrationTest
{
    public class TestModuleBuilder : ModuleBuilder
    {
        public TestModuleBuilder(IServiceCollection services) : base(services) { }

        public void AddAdditionalServices() { }

        public void ConfigureSomething() { }
    }

    [ModuleBuilder(typeof(TestModuleBuilder))]
    public class TestModule : Module
    {
        protected override void ConfigureServices(IServiceCollection services) { }
    }

    [Fact]
    public void TestGeneratedMethods()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureServices(services =>
        {
            services.AddTest(builder =>
            {
                builder.AddAdditionalServices();
                builder.ConfigureSomething();
            });
        });
    }
}
