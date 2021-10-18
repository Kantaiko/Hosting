using Kantaiko.Hosting.Modules;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public class ManagedHostBuilder : IManagedHostBuilder
{
    private IHostLoaderHandler? _hostLoaderHandler;
    private IHostLifecycleHandler? _hostLifecycleHandler;
    private IHostBuilderFactory? _hostBuilderFactory;

    private readonly List<Action<IHostBuilder>> _hostBuilderConfigurators = new();

    public ManagedHostBuilder(string[]? args = null)
    {
        ConstructionContextProvider = new HostBuilderConstructionContextProvider(args);
    }

    protected HostBuilderConstructionContextProvider ConstructionContextProvider { get; }

    public IModuleCollection Modules => ConstructionContextProvider.ModuleCollection;

    public void UseHostLoaderHandler(IHostLoaderHandler hostLoaderHandler)
    {
        _hostLoaderHandler = _hostLoaderHandler is not null
            ? new CombinedHostLoaderHandler(new[] { _hostLoaderHandler, hostLoaderHandler })
            : hostLoaderHandler;
    }

    public void UseHostBuilderFactory(IHostBuilderFactory hostBuilderFactory)
    {
        _hostBuilderFactory = hostBuilderFactory;
    }

    public void ConfigureHostBuilder(Action<IHostBuilder> configureDelegate)
    {
        _hostBuilderConfigurators.Add(configureDelegate);
    }

    public void UseHostLifecycleHandler(IHostLifecycleHandler hostLifecycleHandler)
    {
        _hostLifecycleHandler = _hostLifecycleHandler is not null
            ? new CombinedHostLifecycleHandler(new[] { _hostLifecycleHandler, hostLifecycleHandler })
            : hostLifecycleHandler;
    }

    public virtual IManagedHost Build()
    {
        var hostBuilderFactory = new ManagedHostBuilderHostBuilderFactory(
            _hostBuilderFactory ?? DefaultHostBuilderFactory.Instance, _hostBuilderConfigurators);

        return new ManagedHost(ConstructionContextProvider, _hostLoaderHandler, _hostLifecycleHandler,
            hostBuilderFactory);
    }
}
