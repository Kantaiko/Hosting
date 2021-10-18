namespace Kantaiko.Hosting.Host;

public interface IHostLifecycleHandler
{
    /// <summary>
    /// Invokes on successful host restart.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleRestartAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes on failed host restart.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleRestartFailAsync(IServiceProvider serviceProvider, Exception exception,
        CancellationToken cancellationToken = default);
}
