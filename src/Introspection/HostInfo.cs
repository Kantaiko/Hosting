using System.Reflection;

namespace Kantaiko.Hosting.Introspection;

public class HostInfo
{
    internal HostInfo(Version version, IReadOnlyList<ModuleInfo> modules, IReadOnlyList<ModuleInfo> explicitModules,
        IReadOnlyList<Assembly> assemblies)
    {
        Modules = modules;
        ExplicitModules = explicitModules;
        Assemblies = assemblies;
        Version = version;
    }

    /// <summary>
    /// Version of the Kantaiko Host.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// List of all modules loaded by the host in order of resolution.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Modules { get; }

    /// <summary>
    /// List of modules loaded only explicitly in request order.
    /// </summary>
    public IReadOnlyList<ModuleInfo> ExplicitModules { get; }

    /// <summary>
    /// List of assemblies associated with loaded modules.
    /// </summary>
    public IReadOnlyList<Assembly> Assemblies { get; }
}
