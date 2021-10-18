using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Introspection;
using Kantaiko.Hosting.Loader;
using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Internal;

internal class HostLoader
{
    private readonly ModuleFactory _moduleFactory;

    private readonly Dictionary<Type, LoadedModule> _loadedModules = new();
    private readonly List<(LoadedModule, LoadedModule)> _dependencies = new();

    private readonly ModuleCollection _moduleCollection = new();

    public HostLoader(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _moduleFactory = new ModuleFactory(configuration, hostEnvironment);
    }

    private LoadedModule LoadModule(Type moduleType, LoadedModule? parent = null)
    {
        if (_loadedModules.TryGetValue(moduleType, out var existingModule))
        {
            if (parent is null) return existingModule;

            existingModule.ModuleInfo.AddDependent(parent.ModuleInfo);
            return existingModule;
        }

        var instance = (IModule) _moduleFactory.ConstructModuleInstance(moduleType);

        var moduleInfo = BuildModuleInfo(moduleType);

        if (parent is not null)
            moduleInfo.AddDependent(parent.ModuleInfo);

        var loadedModule = new LoadedModule(moduleInfo, instance);

        _loadedModules[moduleType] = loadedModule;

        instance.ConfigureModules(_moduleCollection);

        var dependencyTypes = _moduleCollection.ModuleTypes.ToArray();
        _moduleCollection.ModuleTypes.Clear();

        var dependencyInfoList = new List<ModuleInfo>();

        foreach (var dependencyType in dependencyTypes)
        {
            var module = LoadModule(dependencyType, loadedModule);
            dependencyInfoList.Add(module.ModuleInfo);

            _dependencies.Add((loadedModule, module));
        }

        moduleInfo.SetDependencies(dependencyInfoList);
        return loadedModule;
    }

    private static ModuleInfo BuildModuleInfo(Type moduleType)
    {
        var moduleInfoOptions = new ModuleInfoOptions();

        foreach (var attribute in moduleType.GetCustomAttributes(true))
        {
            if (attribute is IModuleInfoConfigurationMiddleware middleware)
            {
                middleware.ConfigureInfo(moduleInfoOptions);
            }
        }

        var moduleId = new ModuleIdentifier(moduleType);
        var displayName = moduleInfoOptions.Name ?? moduleType.Name.Replace("Module", "");
        var version = moduleInfoOptions.Version ?? moduleType.Assembly.GetName().Version!;

        return new ModuleInfo(moduleId, displayName, version, moduleInfoOptions.Flags,
            moduleInfoOptions.Properties);
    }

    public LoadedHost Load(ModuleCollection moduleCollection)
    {
        var explicitModuleTypes = moduleCollection.ModuleTypes.ToArray();
        moduleCollection.ModuleTypes.Clear();

        var explicitModules = explicitModuleTypes.Select(type => LoadModule(type)).ToArray();
        var resolvedModules = ResolveModuleOrder().Reverse().ToArray();

        var hostingVersion = typeof(ManagedHost).Assembly.GetName().Version!;

        var resolvedModuleInfos = resolvedModules.Select(x => x.ModuleInfo).ToArray();
        var explicitModuleInfos = explicitModules.Select(x => x.ModuleInfo).ToArray();
        var assemblies = resolvedModuleInfos.Select(x => x.Id.ModuleType.Assembly).Distinct().ToArray();

        var hostInfo = new HostInfo(hostingVersion, resolvedModuleInfos, explicitModuleInfos, assemblies);

        return new LoadedHost(hostInfo, resolvedModules);
    }

    private IReadOnlyList<LoadedModule> ResolveModuleOrder()
    {
        var result = new List<LoadedModule>();

        var s = new HashSet<LoadedModule>(_loadedModules.Values.Where(n =>
            _dependencies.All(e => !e.Item2.Equals(n))));

        while (s.Any())
        {
            var n = s.First();
            s.Remove(n);
            result.Add(n);

            var edgeList = _dependencies.Where(e => e.Item1.Equals(n)).ToList();
            foreach (var e in edgeList)
            {
                var m = e.Item2;
                _dependencies.Remove(e);

                if (_dependencies.All(me => !me.Item2.Equals(m)))
                {
                    s.Add(m);
                }
            }
        }

        if (_dependencies.Any())
        {
            var (dependant, dependency) = _dependencies.First();
            throw new CircularDependencyException(dependant.ModuleInfo, dependency.ModuleInfo);
        }

        return result;
    }
}