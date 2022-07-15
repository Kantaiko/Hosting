using System.Collections.Immutable;
using Kantaiko.Hosting.Modularity.Generator.Models;
using Kantaiko.Hosting.Modularity.Generator.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kantaiko.Hosting.Modularity.Generator;

[Generator]
public class ModuleExtensionGenerator : IIncrementalGenerator
{
    private readonly ModuleExtensionEmitter _emitter = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(RegisterAttributeSources);

        var modules = context.SyntaxProvider
            .CreateSyntaxProvider(CouldBeModuleClass, TryCreateModuleInfo)
            .Where(x => x is not null)
            .Collect();

        context.RegisterSourceOutput(modules, GenerateModuleExtensionMethods);
    }

    private static void RegisterAttributeSources(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("ModuleBuilderAttribute.cs", CodeGenerationUtils.ModuleBuilderAttributeSource);
        context.AddSource("ModuleOptionsAttribute.cs", CodeGenerationUtils.ModuleOptionsAttributeSource);
    }

    private void GenerateModuleExtensionMethods(SourceProductionContext context,
        ImmutableArray<ModuleExtensionInfo?> extensions)
    {
        var validExtensions = ModuleExtensionValidator.GetValidExtensions(context, extensions!);

        foreach (var extension in validExtensions)
        {
            _emitter.Emit(context, extension);
        }
    }

    private const string ModuleBaseClassName = "Module";
    private const string ModuleInterfaceName = "IModule";

    private const string ModuleBaseClassFullName = "Kantaiko.Hosting.Modularity.Module";
    private const string ModuleInterfaceFullName = "Kantaiko.Hosting.Modularity.IModule";

    private const string ModuleBuilderAttributeFullName =
        "Kantaiko.Hosting.Modularity.Generator.ModuleBuilderAttribute";

    private const string ModuleOptionsAttributeFullName =
        "Kantaiko.Hosting.Modularity.Generator.ModuleOptionsAttribute";

    private static bool CouldBeModuleClass(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return false;
        }

        if (classDeclarationSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PrivateKeyword)))
        {
            return false;
        }

        if (classDeclarationSyntax.BaseList is not { } baseList)
        {
            return false;
        }

        return baseList.Types.Any(baseType =>
            SyntaxHelper.ExtractTypeName(baseType.Type) is ModuleBaseClassName or ModuleInterfaceName);
    }

    private static ModuleExtensionInfo? TryCreateModuleInfo(GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax) context.Node;

        var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax,
            cancellationToken: cancellationToken);

        if (declaredSymbol is not { } moduleType)
        {
            return null;
        }

        if (moduleType.BaseType is not { } baseClass)
        {
            return null;
        }

        if (baseClass.ToString() is not ModuleBaseClassFullName or ModuleInterfaceFullName)
        {
            return null;
        }

        var moduleName = NameHelper.ExtractModuleName(moduleType);

        var extensionFlags = ModuleExtensionFlags.None;
        INamedTypeSymbol? additionType = null;

        foreach (var attribute in moduleType.GetAttributes())
        {
            var fullName = attribute.AttributeClass?.ToString();

            switch (fullName)
            {
                case ModuleBuilderAttributeFullName:
                {
                    extensionFlags |= ModuleExtensionFlags.WithBuilder;

                    break;
                }
                case ModuleOptionsAttributeFullName:
                {
                    extensionFlags |= ModuleExtensionFlags.WithOptions;

                    break;
                }
                default:
                {
                    continue;
                }
            }

            var value = attribute.ConstructorArguments.FirstOrDefault().Value;

            additionType = value as INamedTypeSymbol;
        }

        return new ModuleExtensionInfo(moduleType, moduleName, extensionFlags, additionType);
    }
}
