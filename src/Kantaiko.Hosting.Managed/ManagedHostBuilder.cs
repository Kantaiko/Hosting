using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

public class ManagedHostBuilder : IManagedHostBuilder
{
    private IHostBuilderFactory? _hostBuilderFactory;

    private readonly List<Action<IHostBuilder>> _configureDelegates = new();

    public IManagedHostBuilder UseHostBuilderFactory(IHostBuilderFactory hostBuilderFactory)
    {
        _hostBuilderFactory = hostBuilderFactory;

        return this;
    }

    public IManagedHostBuilder ConfigureHostBuilder(Action<IHostBuilder> configureDelegate)
    {
        _configureDelegates.Add(configureDelegate);

        return this;
    }

    public IManagedHost Build()
    {
        var hostBuilderFactory = _hostBuilderFactory;

        if (_configureDelegates.Count > 0)
        {
            hostBuilderFactory ??= new DefaultHostBuilderFactory();
            hostBuilderFactory = new DelegateHostBuilderFactory(hostBuilderFactory, _configureDelegates);
        }

        var hostBuilderFactoryProvider = hostBuilderFactory is null
            ? null
            : new SingleHostBuilderFactoryProvider(hostBuilderFactory);

        return new ManagedHost(hostBuilderFactoryProvider);
    }
}
