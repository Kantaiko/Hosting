namespace Kantaiko.Hosting.Host;

public class DefaultHostLifecycleHandler : IHostLifecycleHandler
{
    public virtual Task HandleRestartAsync(IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default) => Task.CompletedTask;

    public virtual Task HandleRestartFailAsync(IServiceProvider serviceProvider, Exception exception,
        CancellationToken cancellationToken = default) => Task.CompletedTask;

    private static DefaultHostLifecycleHandler? _instance;
    public static DefaultHostLifecycleHandler Instance => _instance ??= new DefaultHostLifecycleHandler();
}
