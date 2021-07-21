using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DiffEngine;
using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Internal;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using VerifyXunit;
using Xunit;

namespace Kantaiko.Hosting.Tests
{
    [UsesVerify]
    public class HostLoaderTest
    {
        private class TestModuleA : IModule { }

        private class TestModuleB : IModule
        {
            public void ConfigureModules(IModuleCollection modules)
            {
                modules.Add<TestModuleA>();
            }
        }

        private class TestModuleC : IModule
        {
            public void ConfigureModules(IModuleCollection modules)
            {
                modules.Add<TestModuleA>();
                modules.Add<TestModuleB>();
            }
        }

        [Fact]
        public async Task ShouldLoadHostWithCorrectModuleOrder()
        {
            var hostLoader = CreateHostLoader();
            var moduleCollection = new ModuleCollection();

            moduleCollection.Add<TestModuleC>();
            moduleCollection.Add<TestModuleB>();

            var loadedHost = hostLoader.Load(moduleCollection);

            DiffTools.UseOrder(DiffTool.Rider, DiffTool.VisualStudioCode);

            await Verifier.Verify(loadedHost)
                .ModifySerialization(x => x.IgnoreMembers("Version", "Assemblies"))
                .UseDirectory("__snapshots__");
        }

        private class TestModuleD : IModule
        {
            public void ConfigureModules(IModuleCollection modules)
            {
                modules.Add<TestModuleF>();
            }
        }

        private class TestModuleE : IModule
        {
            public void ConfigureModules(IModuleCollection modules)
            {
                modules.Add<TestModuleD>();
            }
        }

        private class TestModuleF : IModule
        {
            public void ConfigureModules(IModuleCollection modules)
            {
                modules.Add<TestModuleE>();
            }
        }

        [Fact]
        public void ShouldReportCircularDependency()
        {
            var hostLoader = CreateHostLoader();
            var moduleCollection = new ModuleCollection();

            moduleCollection.Add<TestModuleF>();

            void Action()
            {
                hostLoader.Load(moduleCollection);
            }

            var exception = Assert.Throws<CircularDependencyException>(Action);
            Assert.Equal(typeof(TestModuleD), exception.First.Id.ModuleType);
            Assert.Equal(typeof(TestModuleF), exception.Second.Id.ModuleType);
        }

        private const string TestCustomProperty = nameof(TestCustomProperty);

        private class CustomPropertyAttribute : Attribute, IModuleInfoConfigurationMiddleware
        {
            private readonly int _value;

            public CustomPropertyAttribute(int value)
            {
                _value = value;
            }

            public void ConfigureInfo(ModuleInfoOptions options)
            {
                options.Properties[TestCustomProperty] = _value;
            }
        }

        [ModuleName("G")]
        [ModuleVersion("42.42.42")]
        [ModuleFlags(ModuleFlags.Library)]
        [CustomProperty(42)]
        private class TestModuleG : IModule { }

        [Fact]
        public void ShouldLoadModuleWithInfo()
        {
            var hostLoader = CreateHostLoader();
            var moduleCollection = new ModuleCollection();

            moduleCollection.Add<TestModuleG>();

            var loadedHost = hostLoader.Load(moduleCollection);
            var module = loadedHost.HostInfo.Modules[0];

            Assert.Equal("G", module.DisplayName);
            Assert.Equal(Version.Parse("42.42.42"), module.Version);
            Assert.Equal(ModuleFlags.Library, module.Flags);
            Assert.Equal(42, module.Properties[TestCustomProperty]);
        }

        private static HostLoader CreateHostLoader()
        {
            var configuration = new ConfigurationRoot(new Collection<IConfigurationProvider>());
            var hostingEnvironment = new HostingEnvironment();
            var hostLoader = new HostLoader(configuration, hostingEnvironment);
            return hostLoader;
        }
    }
}
