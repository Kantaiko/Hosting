using Kantaiko.Properties;

namespace Kantaiko.Hosting.Managed.Runtime;

public interface IRuntimeHostState : IPropertyContainer
{
    bool RestartRequested { get; }
    DateTime RestartRequestedAt { get; }
    bool HostRestarted { get; }
    bool RestartFailed { get; }
    DateTime RestartCompletedAt { get; }
    Exception? StartupException { get; }
}
