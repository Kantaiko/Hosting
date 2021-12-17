using Kantaiko.Properties;

namespace Kantaiko.Hosting.Modularity.Introspection;

public class ModuleInfo
{
    internal ModuleInfo(ModuleIdentifier id, ModuleMetadata metadata,
        IReadOnlyList<ModuleInfo> dependencies)
    {
        Id = id;
        Metadata = metadata;
        Dependencies = dependencies;
    }

    public ModuleMetadata Metadata { get; }

    /// <summary>
    /// The unique identifier of the module.
    /// </summary>
    public ModuleIdentifier Id { get; }

    /// <summary>
    /// The display name of the module. Should be used in admin interfaces.
    /// By default, it is a module class name without "Module" suffix.
    /// </summary>
    public string DisplayName => Metadata.DisplayName;

    /// <summary>
    /// Version of the module.
    /// </summary>
    public Version Version => Metadata.Version;

    /// <summary>
    /// Flags, representing some useful information about the module.
    /// </summary>
    public ModuleFlags Flags => Metadata.Flags;

    /// <summary>
    /// Collection of user-defined module properties.
    /// </summary>
    public IReadOnlyPropertyCollection Properties => Metadata.Properties;

    /// <summary>
    /// Dependencies, explicitly requested by the module.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Dependencies { get; }

    /// <summary>
    /// All modules that have requested this module to load.
    /// Empty, if this module was loaded directly by the host.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Dependents { get; internal set; } = null!;
}
