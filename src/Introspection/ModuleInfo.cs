using Kantaiko.Hosting.Modules;

namespace Kantaiko.Hosting.Introspection;

public class ModuleInfo
{
    internal ModuleInfo(ModuleIdentifier id, string displayName, Version version, ModuleFlags flags,
        Dictionary<string, object> properties)
    {
        Id = id;
        Flags = flags;
        Properties = properties;
        DisplayName = displayName;
        Version = version;
    }

    /// <summary>
    /// The unique identifier of the module.
    /// </summary>
    public ModuleIdentifier Id { get; }

    /// <summary>
    /// The display name of the module. Should be used in admin interfaces.
    /// By default, it is a module class name without "Module" suffix.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Version of the module.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Flags, representing some useful information about the module.
    /// </summary>
    public ModuleFlags Flags { get; }

    /// <summary>
    /// Collection of user-defined module properties.
    /// </summary>
    public Dictionary<string, object> Properties { get; }

    /// <summary>
    /// Dependencies, explicitly requested by the module.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Dependencies { get; private set; } = null!;

    /// <summary>
    /// All modules that have requested this module to load.
    /// Empty, if this module was loaded directly by the host.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Dependents { get; private set; } = Array.Empty<ModuleInfo>();

    internal void SetDependencies(IReadOnlyList<ModuleInfo> dependencies)
    {
        Dependencies = dependencies;
    }

    internal void AddDependent(ModuleInfo moduleInfo)
    {
        Dependents = Dependents.Append(moduleInfo).ToArray();
    }
}
