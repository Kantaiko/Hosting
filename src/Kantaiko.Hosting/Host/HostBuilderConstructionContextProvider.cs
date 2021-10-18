using Kantaiko.Hosting.Internal;

namespace Kantaiko.Hosting.Host;

public class HostBuilderConstructionContextProvider : IHostConstructionContextProvider
{
    private readonly string[]? _args;

    public HostBuilderConstructionContextProvider(string[]? args)
    {
        _args = args;
    }

    public ModuleCollection ModuleCollection { get; } = new();

    public HostConstructionContext GetHostConstructionContext()
    {
        return new HostConstructionContext(_args, ModuleCollection.ModuleTypes);
    }
}
