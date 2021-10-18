namespace Kantaiko.Hosting.Runtime;

public interface IRuntimeHostManager
{
    /// <summary>
    /// Requests host restart.
    /// If allowed, the host will be stopped, recreated and restarted, all modules will be reloaded.
    /// </summary>
    void Restart();

    /// <summary>
    /// Requests host stop.
    /// If allowed, the host will be stopped and destroyed, app will be finished with zero code.
    /// </summary>
    void Stop();
}
