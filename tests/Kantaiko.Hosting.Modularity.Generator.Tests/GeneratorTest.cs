namespace Kantaiko.Hosting.Modularity.Generator.Tests;

[UsesVerify]
public class GeneratorTest
{
    [Fact]
    public Task ShouldGenerateForSimpleModule()
    {
        const string source = @"
            using Kantaiko.Hosting.Modularity;

            namespace Test;

            public class TestModule : Module { }
        ";

        return TestHelper.GenerateAndVerify(source);
    }

    [Fact]
    public Task ShouldGenerateForModuleWithBuilder()
    {
        const string source = @"
            using Kantaiko.Hosting.Modularity;
            using Kantaiko.Hosting.Modularity.Generator;
            using Microsoft.Extensions.DependencyInjection;

            namespace Test;

            public class TestModuleBuilder : ModuleBuilder
            {
                public TestModuleBuilder(IServiceCollection services) : base(services) { }
            }

            [ModuleBuilder(typeof(TestModuleBuilder))]
            public class TestModule : Module { }
        ";

        return TestHelper.GenerateAndVerify(source);
    }

    [Fact]
    public Task ShouldGenerateForModuleWithOptions()
    {
        const string source = @"
            using Kantaiko.Hosting.Modularity;
            using Kantaiko.Hosting.Modularity.Generator;
            using Microsoft.Extensions.DependencyInjection;

            namespace Test;

            public class TestModuleOptions { }

            [ModuleOptions(typeof(TestModuleOptions))]
            public class TestModule : Module { }
        ";

        return TestHelper.GenerateAndVerify(source);
    }

    [Fact]
    public Task ShouldGenerateCodeForModuleWithCustomBaseClass()
    {
        const string source = @"
            using Kantaiko.Hosting.Modularity;
            using Kantaiko.Hosting.Modularity.Generator;
            using Microsoft.Extensions.DependencyInjection;

            namespace Test;

            public abstract class CustomModule : Module { }

            public class TestModule : CustomModule { }
        ";

        return TestHelper.GenerateAndVerify(source);
    }

    [Fact]
    public Task ShouldReportDuplicateModuleNames()
    {
        const string source = @"
            using Kantaiko.Hosting.Modularity;
            using Kantaiko.Hosting.Modularity.Generator;
            using Microsoft.Extensions.DependencyInjection;

            namespace Test
            {
                public class TestModule : Module { }
            }

            namespace Test2
            {
                public class TestModule : Module { }
            }
        ";

        return TestHelper.GenerateAndVerify(source);
    }

    [Fact]
    public Task ShouldReportBothOptionsAndModuleBuilderUsage()
    {
        const string source = @"
            using Kantaiko.Hosting.Modularity;
            using Kantaiko.Hosting.Modularity.Generator;
            using Microsoft.Extensions.DependencyInjection;

            namespace Test;

            public class TestModuleOptions { }

            public class TestModuleBuilder : ModuleBuilder
            {
                public TestModuleBuilder(IServiceCollection services) : base(services) { }
            }

            [ModuleOptions(typeof(TestModuleOptions))]
            [ModuleBuilder(typeof(TestModuleBuilder))]
            public class TestModule : Module { }
        ";

        return TestHelper.GenerateAndVerify(source);
    }
}
