namespace Kantaiko.Hosting.Host;

public class CombinedHostLifecycleHandler : IHostLifecycleHandler
{
    private readonly IEnumerable<IHostLifecycleHandler> _handlers;

    public CombinedHostLifecycleHandler(IEnumerable<IHostLifecycleHandler> handlers)
    {
        _handlers = handlers;
    }

    public async Task HandleRestartAsync(IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        foreach (var hostLifecycleHandler in _handlers)
        {
            await hostLifecycleHandler.HandleRestartAsync(serviceProvider, cancellationToken);
        }
    }

    public async Task HandleRestartFailAsync(IServiceProvider serviceProvider, Exception exception,
        CancellationToken cancellationToken = default)
    {
        foreach (var hostLifecycleHandler in _handlers)
        {
            await hostLifecycleHandler.HandleRestartFailAsync(serviceProvider, exception, cancellationToken);
        }
    }
}
