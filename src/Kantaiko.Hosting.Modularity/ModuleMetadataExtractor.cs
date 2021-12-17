using System.Reflection;
using Kantaiko.Hosting.Modularity.Introspection;

namespace Kantaiko.Hosting.Modularity;

public static class ModuleMetadataExtractor
{
    public static ModuleMetadata Extract(IModule module)
    {
        var moduleType = module.GetType();

        var moduleAttribute = moduleType
            .GetCustomAttributes()
            .OfType<ModuleAttribute>()
            .FirstOrDefault();

        string? displayName = null;
        Version? version = null;
        string? description = null;
        ModuleFlags flags = default;

        if (moduleAttribute is not null)
        {
            displayName = moduleAttribute.DisplayName;
            description = moduleAttribute.Description;
            flags = moduleAttribute.Flags;

            if (moduleAttribute.Version is not null)
            {
                version = Version.Parse(moduleAttribute.Version);
            }
        }

        displayName ??= NormalizeModuleName(moduleType.Name);
        version ??= moduleType.Assembly.GetName().Version ?? new Version();

        return new ModuleMetadata(displayName, version, description, flags);
    }

    private static string NormalizeModuleName(string moduleName)
    {
        if (!moduleName.EndsWith("Module")) return moduleName;

        var modulePostfixIndex = moduleName.LastIndexOf("Module", StringComparison.Ordinal);
        return moduleName.Substring(0, modulePostfixIndex);
    }
}
