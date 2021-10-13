using Kantaiko.Hosting.Modules;

namespace Kantaiko.Hosting.Host;

public class ManagedHostBuilder
{
    protected string[]? Args { get; }

    public ManagedHostBuilder(string[]? args = null)
    {
        Args = args;
    }

    protected HostBuilderConstructionContextProvider ConstructionContextProvider { get; } = new();

    public IModuleCollection Modules => ConstructionContextProvider.ModuleCollection;

    public virtual IManagedHost Build()
    {
        return new ManagedHost(Args, ConstructionContextProvider);
    }
}