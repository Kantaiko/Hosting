//HintName: ModuleOptionsAttribute.cs
using System;

namespace Kantaiko.Hosting.Modularity.Generator;

[AttributeUsage(AttributeTargets.Class)]
internal class ModuleOptionsAttribute : Attribute
{
    public ModuleOptionsAttribute(Type optionsType)
    {
        OptionsType = optionsType;
    }

    public Type OptionsType { get; }
}