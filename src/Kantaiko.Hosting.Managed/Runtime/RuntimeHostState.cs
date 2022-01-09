using Kantaiko.Properties;

namespace Kantaiko.Hosting.Managed.Runtime;

internal class RuntimeHostState : IRuntimeHostState
{
    public bool RestartRequested { get; set; }
    public DateTime RestartRequestedAt { get; set; }
    public IPropertyCollection Properties { get; set; } = new PropertyCollection();

    public bool HostRestarted { get; set; }
    public bool RestartFailed { get; set; }
    public DateTime RestartCompletedAt { get; set; }
    public Exception? StartupException { get; set; }
}
