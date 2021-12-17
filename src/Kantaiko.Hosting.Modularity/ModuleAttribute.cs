using Kantaiko.Hosting.Modularity.Introspection;

namespace Kantaiko.Hosting.Modularity;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleAttribute : Attribute
{
    public string? DisplayName { get; init; }
    public string? Version { get; init; }

    public string? Description { get; init; }

    public ModuleFlags Flags { get; init; }
}
