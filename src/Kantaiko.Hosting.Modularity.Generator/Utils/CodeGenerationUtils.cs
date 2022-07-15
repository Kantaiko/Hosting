namespace Kantaiko.Hosting.Modularity.Generator.Utils;

internal static class CodeGenerationUtils
{
    public const string ModuleBuilderAttributeSource = @"using System;

namespace Kantaiko.Hosting.Modularity.Generator;

[AttributeUsage(AttributeTargets.Class)]
internal class ModuleBuilderAttribute : Attribute
{
    public ModuleBuilderAttribute(Type moduleBuilderType)
    {
        ModuleBuilderType = moduleBuilderType;
    }

    public Type ModuleBuilderType { get; }
}";

    public const string ModuleOptionsAttributeSource = @"using System;

namespace Kantaiko.Hosting.Modularity.Generator;

[AttributeUsage(AttributeTargets.Class)]
internal class ModuleOptionsAttribute : Attribute
{
    public ModuleOptionsAttribute(Type optionsType)
    {
        OptionsType = optionsType;
    }

    public Type OptionsType { get; }
}";
}
