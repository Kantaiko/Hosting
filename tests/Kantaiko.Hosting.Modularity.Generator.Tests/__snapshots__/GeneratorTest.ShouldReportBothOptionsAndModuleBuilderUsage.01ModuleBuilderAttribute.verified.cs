//HintName: ModuleBuilderAttribute.cs
using System;

namespace Kantaiko.Hosting.Modularity.Generator;

[AttributeUsage(AttributeTargets.Class)]
internal class ModuleBuilderAttribute : Attribute
{
    public ModuleBuilderAttribute(Type moduleBuilderType)
    {
        ModuleBuilderType = moduleBuilderType;
    }

    public Type ModuleBuilderType { get; }
}