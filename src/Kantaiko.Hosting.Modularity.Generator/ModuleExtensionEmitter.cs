using Kantaiko.Hosting.Modularity.Generator.Models;
using Kantaiko.Hosting.Modularity.Generator.Utils;
using Microsoft.CodeAnalysis;

namespace Kantaiko.Hosting.Modularity.Generator;

internal class ModuleExtensionEmitter
{
    private readonly IndentedStringBuilder _builder = new(512);
    private readonly NamespaceImportEmitter _importEmitter = new();

    public void Emit(SourceProductionContext context, ModuleExtensionInfo extensionInfo)
    {
        var content = Emit(extensionInfo);

        context.AddSource($"{extensionInfo.ModuleName}.ServiceCollectionExtensions.cs", content);

        Clear();
    }

    private string Emit(ModuleExtensionInfo extensionInfo)
    {
        _importEmitter.AddNamespace("Microsoft.Extensions.DependencyInjection");
        _importEmitter.AddNamespace("Kantaiko.Hosting.Modularity");

        _builder.AppendIndentedFormatLine("namespace {0};", extensionInfo.ModuleType.ContainingNamespace);
        _builder.AppendLine();

        _builder.AppendIndentedFormatLine(
            "public static class {0}ServiceCollectionExtensions",
            extensionInfo.ModuleName
        );

        _builder.AppendOpenBrace();

        var methodName = $"Add{extensionInfo.ModuleName}";

        _builder.AppendIndentedFormat(
            "public static IServiceCollection {0}(this IServiceCollection services",
            methodName
        );

        if (extensionInfo.Flags is not ModuleExtensionFlags.None)
        {
            _builder.AppendFormat(
                ", Action<{0}>? configureDelegate = null",
                _importEmitter.GetTypeExpression(extensionInfo.AdditionType!)
            );
        }

        _builder.AppendLine(')');

        _builder.AppendOpenBrace();

        _builder.AppendIndentedFormatLine(
            "services.AddModule<{0}>();",
            _importEmitter.GetTypeExpression(extensionInfo.ModuleType)
        );

        _builder.AppendLine();

        switch (extensionInfo.Flags)
        {
            case ModuleExtensionFlags.WithOptions:
            {
                _builder.AppendIndentedLine("services.TryConfigure(configureDelegate);");

                _builder.AppendLine();
                break;
            }
            case ModuleExtensionFlags.WithBuilder:
            {
                _builder.AppendIndentedFormatLine(
                    "configureDelegate?.Invoke(new {0}(services));",
                    _importEmitter.GetTypeExpression(extensionInfo.AdditionType!)
                );

                _builder.AppendLine();
                break;
            }
        }

        _builder.AppendIndentedLine("return services;");

        _builder.AppendCloseBrace();
        _builder.AppendCloseBrace();

        var sb = _builder.ToBuilder();

        _importEmitter.Emit(ref sb);

        return sb.ToString();
    }

    private void Clear()
    {
        _builder.Clear();
        _importEmitter.Clear();
    }
}
