using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public class ManagedHostBuilderHostBuilderFactory : IHostBuilderFactory
{
    private readonly IHostBuilderFactory _outerHostBuilderFactory;
    private readonly IReadOnlyList<Action<IHostBuilder>> _hostBuilderConfigurators;

    public ManagedHostBuilderHostBuilderFactory(IHostBuilderFactory outerHostBuilderFactory,
        IReadOnlyList<Action<IHostBuilder>> hostBuilderConfigurators)
    {
        _outerHostBuilderFactory = outerHostBuilderFactory;
        _hostBuilderConfigurators = hostBuilderConfigurators;
    }

    public IHostBuilder CreateHostBuilder(HostConstructionContext constructionContext)
    {
        var hostBuilder = _outerHostBuilderFactory.CreateHostBuilder(constructionContext);

        foreach (var hostBuilderConfigurator in _hostBuilderConfigurators)
        {
            hostBuilderConfigurator.Invoke(hostBuilder);
        }

        return hostBuilder;
    }
}
