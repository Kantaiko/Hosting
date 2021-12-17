using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

public class DelegateHostBuilderFactory : IHostBuilderFactory
{
    private readonly IHostBuilderFactory _origin;
    private readonly IReadOnlyList<Action<IHostBuilder>> _configureDelegates;

    public DelegateHostBuilderFactory(IHostBuilderFactory origin,
        IReadOnlyList<Action<IHostBuilder>> configureDelegates)
    {
        _origin = origin;
        _configureDelegates = configureDelegates;
    }

    public IHostBuilder CreateHostBuilder()
    {
        var hostBuilder = _origin.CreateHostBuilder();

        foreach (var configureDelegate in _configureDelegates)
        {
            configureDelegate(hostBuilder);
        }

        return hostBuilder;
    }
}
