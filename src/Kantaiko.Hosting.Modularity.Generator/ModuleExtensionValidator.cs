using System.Collections.Immutable;
using Kantaiko.Hosting.Modularity.Generator.Models;
using Microsoft.CodeAnalysis;

namespace Kantaiko.Hosting.Modularity.Generator;

internal static class ModuleExtensionValidator
{
    private static readonly DiagnosticDescriptor ModuleNameConflictRule = new(
        "Kantaiko:Modularity:ModuleNameConflict",
        "Module name conflict",
        "Project cannot contain multiple modules with the same name",
        "Modularity",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor ModuleAllowOnlyOptionsOrBuilderRule = new(
        "Kantaiko:Modularity:ModuleAllowOnlyOptionsOrBuilder",
        "Module allow only options or builder",
        "For one module you can specify either options or builder",
        "Modularity",
        DiagnosticSeverity.Error,
        true
    );

    private static readonly DiagnosticDescriptor UnavailableModuleOrOptionsType = new(
        "Kantaiko:Modularity:UnavailableModuleOrOptionsType",
        "Unavailable module or options type",
        "Failed to resolve module or options type",
        "Modularity",
        DiagnosticSeverity.Error,
        true
    );

    public static IEnumerable<ModuleExtensionInfo> GetValidExtensions(SourceProductionContext context,
        ImmutableArray<ModuleExtensionInfo> extensions)
    {
        var duplicates = extensions.GroupBy(x => x?.ModuleName)
            .Where(x => x.Count() > 1)
            .ToArray();

        if (duplicates.Length > 0)
        {
            foreach (var duplicateGroup in duplicates)
            {
                foreach (var extensionInfo in duplicateGroup)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        ModuleNameConflictRule,
                        extensionInfo!.ModuleType.Locations.FirstOrDefault()
                    ));
                }
            }
        }

        var duplicateModuleNames = duplicates.Select(x => x.Key).ToArray();

        foreach (var extensionInfo in extensions)
        {
            if (duplicateModuleNames.Contains(extensionInfo!.ModuleName))
            {
                continue;
            }

            if (extensionInfo.Flags is ModuleExtensionFlags.Both)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    ModuleAllowOnlyOptionsOrBuilderRule,
                    extensionInfo.ModuleType.Locations.FirstOrDefault()
                ));

                continue;
            }

            if (extensionInfo.Flags is not ModuleExtensionFlags.None && extensionInfo.AdditionType is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    UnavailableModuleOrOptionsType,
                    extensionInfo.ModuleType.Locations.FirstOrDefault()
                ));

                continue;
            }

            yield return extensionInfo;
        }
    }
}
