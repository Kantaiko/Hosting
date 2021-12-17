using System.Collections.Immutable;
using System.Reflection;

namespace Kantaiko.Hosting.Modularity.Introspection;

public class HostInfo
{
    internal HostInfo()
    {
        Modules = ImmutableArray<ModuleInfo>.Empty;
        ExplicitModules = ImmutableArray<ModuleInfo>.Empty;
        Assemblies = ImmutableHashSet<Assembly>.Empty;
    }

    internal HostInfo(IReadOnlyList<ModuleInfo> modules,
        IReadOnlyList<ModuleInfo> explicitModules,
        IReadOnlySet<Assembly> assemblies)
    {
        Modules = modules;
        ExplicitModules = explicitModules;
        Assemblies = assemblies;
    }

    /// <summary>
    /// List of all modules loaded by the host in order of resolution.
    /// </summary>
    public IReadOnlyList<ModuleInfo> Modules { get; }

    /// <summary>
    /// List of modules loaded only explicitly in request order.
    /// </summary>
    public IReadOnlyList<ModuleInfo> ExplicitModules { get; }

    /// <summary>
    /// Set of assemblies associated with loaded modules.
    /// </summary>
    public IReadOnlySet<Assembly> Assemblies { get; }
}
