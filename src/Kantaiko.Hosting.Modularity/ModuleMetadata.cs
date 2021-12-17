using Kantaiko.Hosting.Modularity.Introspection;
using Kantaiko.Properties;
using Kantaiko.Properties.Immutable;

namespace Kantaiko.Hosting.Modularity;

public class ModuleMetadata
{
    public ModuleMetadata(string displayName, Version version, string? description = null,
        ModuleFlags flags = ModuleFlags.None, IReadOnlyPropertyCollection? properties = null)
    {
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(version);

        DisplayName = displayName;
        Version = version;
        Description = description;
        Flags = flags;
        Properties = properties ?? new ImmutablePropertyCollection();
    }

    public string DisplayName { get; init; }
    public Version Version { get; init; }
    public string? Description { get; init; }
    public ModuleFlags Flags { get; init; }
    public IReadOnlyPropertyCollection Properties { get; init; }
}
