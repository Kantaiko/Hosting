namespace Kantaiko.Hosting.Managed.Runtime;

internal class RuntimeHostState : IRuntimeHostState
{
    public bool RestartRequested { get; set; }
    public DateTime RestartRequestedAt { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();

    public bool HostRestarted { get; set; }
    public bool RestartFailed { get; set; }
    public DateTime RestartCompletedAt { get; set; }
    public Exception? StartupException { get; set; }
}
