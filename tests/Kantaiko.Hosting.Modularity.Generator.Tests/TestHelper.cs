using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Kantaiko.Hosting.Modularity.Generator.Tests;

public static class TestHelper
{
    public static Task GenerateAndVerify(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IModule).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references
        );

        var generator = new ModuleExtensionGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        var result = driver.RunGenerators(compilation);

        return Verify(result)
            .DisableDiff()
            .UseDirectory("__snapshots__");
    }
}
