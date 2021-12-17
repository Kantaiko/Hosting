using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Modularity.Internal;

internal class ModuleManager
{
    private readonly Dictionary<Type, ModuleDescriptor> _moduleDescriptors = new();
    private readonly Stack<ModuleDescriptor> _descriptorStack = new();
    private readonly List<ModuleDescriptor> _explicitModuleDescriptors = new();

    private readonly ModuleFactory _moduleFactory;
    private readonly IServiceCollection _services;

    public ModuleManager(IServiceCollection services)
    {
        _services = services;

        var hostBuilderContext = ServiceCollectionHelper.GetHostBuilderContext(services);
        _moduleFactory = new ModuleFactory(hostBuilderContext.Configuration, hostBuilderContext.HostingEnvironment);
    }

    public bool IsRegistered(Type moduleType)
    {
        return _moduleDescriptors.ContainsKey(moduleType);
    }

    public void AddModule(Type moduleType)
    {
        if (IsRegistered(moduleType)) return;

        var dependentModule = _descriptorStack.Count > 0 ? _descriptorStack.Peek() : null;
        var instance = _moduleFactory.ConstructModuleInstance(moduleType);

        var metadata = instance.GetMetadata();

        var descriptor = new ModuleDescriptor(moduleType, metadata);
        _moduleDescriptors[moduleType] = descriptor;

        if (dependentModule is null)
        {
            _explicitModuleDescriptors.Add(descriptor);
        }
        else
        {
            dependentModule.AppendDependency(descriptor);
        }

        _descriptorStack.Push(descriptor);

        instance.ConfigureServices(_services);

        _descriptorStack.Pop();
    }

    public IReadOnlyDictionary<Type, ModuleDescriptor> ModuleDescriptors => _moduleDescriptors;
    public IReadOnlyList<ModuleDescriptor> ExplicitModuleDescriptors => _explicitModuleDescriptors;
}
