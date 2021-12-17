using System.Collections.Immutable;
using System.Reflection;
using Kantaiko.Hosting.Modularity.Introspection;

namespace Kantaiko.Hosting.Modularity.Internal;

internal class HostInfoFactory
{
    private readonly ModuleManager _moduleManager;
    private readonly HashSet<Assembly> _assemblies = new();
    private readonly Dictionary<Type, ModuleInfo> _loadedModules;

    public HostInfoFactory(ModuleManager moduleManager)
    {
        _moduleManager = moduleManager;
        _loadedModules = new Dictionary<Type, ModuleInfo>(moduleManager.ModuleDescriptors.Count);
    }

    private ModuleInfo CreateModuleInfo(ModuleDescriptor descriptor)
    {
        if (!_loadedModules.TryGetValue(descriptor.ModuleType, out var moduleInfo))
        {
            var moduleId = new ModuleIdentifier(descriptor.ModuleType);

            var dependencies = descriptor.DependencyModules?
                .Select(CreateModuleInfo).ToImmutableArray() ?? ImmutableArray<ModuleInfo>.Empty;

            moduleInfo = new ModuleInfo(moduleId, descriptor.Metadata, dependencies);
            _loadedModules[descriptor.ModuleType] = moduleInfo;

            _assemblies.Add(descriptor.ModuleType.Assembly);
        }

        return moduleInfo;
    }

    public HostInfo CreateHostInfo()
    {
        var explicitModules = _moduleManager.ExplicitModuleDescriptors
            .Select(CreateModuleInfo)
            .ToImmutableArray();

        var modules = _loadedModules.Values.ToImmutableArray();

        foreach (var descriptor in _moduleManager.ModuleDescriptors.Values)
        {
            var moduleInfo = _loadedModules[descriptor.ModuleType];

            if (descriptor.DependentModules is null)
            {
                moduleInfo.Dependents = ImmutableArray<ModuleInfo>.Empty;
                continue;
            }

            var dependents = descriptor.DependentModules.Select(x => _loadedModules[x.ModuleType]);
            moduleInfo.Dependents = dependents.ToImmutableArray();
        }

        return new HostInfo(modules, explicitModules, _assemblies);
    }
}
