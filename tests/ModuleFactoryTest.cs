using System.Collections.ObjectModel;
using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Internal;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Xunit;

namespace Kantaiko.Hosting.Tests;

public class ModuleFactoryTest
{
    [Fact]
    public void ShouldCreateModuleInstanceWithAdditionalParameters()
    {
        var moduleFactory = CreateModuleFactory();

        var instance = (TestModuleA) moduleFactory.ConstructModuleInstance(typeof(TestModuleA));
        Assert.NotNull(instance.Configuration);
        Assert.NotNull(instance.Environment);
    }

    [Fact]
    public void ShouldReportInvalidModuleConstructorParameter()
    {
        var moduleFactory = CreateModuleFactory();

        void Action()
        {
            moduleFactory.ConstructModuleInstance(typeof(TestModuleB));
        }

        Assert.Throws<ModuleConstructionException>(Action);
    }

    [Fact]
    public void ShouldReportMultipleModuleConstructors()
    {
        var moduleFactory = CreateModuleFactory();

        void Action()
        {
            moduleFactory.ConstructModuleInstance(typeof(TestModuleC));
        }

        Assert.Throws<ModuleConstructionException>(Action);
    }

    [Fact]
    public void ShouldReportInaccessibleModuleConstructor()
    {
        var moduleFactory = CreateModuleFactory();

        void Action()
        {
            moduleFactory.ConstructModuleInstance(typeof(TestModuleD));
        }

        Assert.Throws<ModuleConstructionException>(Action);
    }

    private class TestModuleA : IModule
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment Environment { get; }

        public TestModuleA(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
    }

    private class TestModuleB
    {
        public TestModuleB(string test) { }
    }

    private class TestModuleC
    {
        public TestModuleC(IConfiguration configuration) { }

        public TestModuleC(IHostEnvironment hostEnvironment) { }
    }

    private class TestModuleD
    {
        private TestModuleD() { }
    }

    private static ModuleFactory CreateModuleFactory()
    {
        var configuration = new ConfigurationRoot(new Collection<IConfigurationProvider>());
        var hostingEnvironment = new HostingEnvironment();
        return new ModuleFactory(configuration, hostingEnvironment);
    }
}