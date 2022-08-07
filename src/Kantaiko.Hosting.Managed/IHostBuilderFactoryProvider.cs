namespace Kantaiko.Hosting.Managed;

public interface IHostBuilderFactoryProvider
{
    /// <summary>
    /// Indicates that this factory provider may return different factories and managed host should get new factory on each restart.
    /// It also should try to restore a previous host using previously returned factory on startup failure.
    /// </summary>
    bool Volatile { get; }

    Task<IHostBuilderFactory> GetHostBuilderFactoryAsync(CancellationToken cancellationToken);
}
