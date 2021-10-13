using Kantaiko.Hosting.Internal;

namespace Kantaiko.Hosting.Host;

public class HostBuilderConstructionContextProvider : IHostConstructionContextProvider
{
    public ModuleCollection ModuleCollection { get; } = new();

    public HostConstructionContext GetHostConstructionContext()
    {
        return new(ModuleCollection.ModuleTypes);
    }
}