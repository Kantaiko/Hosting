namespace Kantaiko.Hosting.Modularity.Internal;

internal class ModuleDescriptor
{
    public ModuleDescriptor(Type moduleType, ModuleMetadata metadata)
    {
        ModuleType = moduleType;
        Metadata = metadata;
    }

    public Type ModuleType { get; }
    public ModuleMetadata Metadata { get; }

    public IEnumerable<ModuleDescriptor>? DependentModules { get; set; }
    public IEnumerable<ModuleDescriptor>? DependencyModules { get; set; }

    public void AppendDependency(ModuleDescriptor descriptor)
    {
        DependencyModules = AppendDescriptor(DependencyModules, descriptor);
        descriptor.DependentModules = AppendDescriptor(descriptor.DependentModules, this);
    }

    private static IEnumerable<ModuleDescriptor> AppendDescriptor(
        IEnumerable<ModuleDescriptor>? descriptors,
        ModuleDescriptor descriptor)
    {
        return descriptors is null
            ? Enumerable.Repeat(descriptor, 1)
            : descriptors.Append(descriptor);
    }
}
