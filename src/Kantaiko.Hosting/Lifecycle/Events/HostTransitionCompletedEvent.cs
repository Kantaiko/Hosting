using Kantaiko.Hosting.Managed.Runtime;

namespace Kantaiko.Hosting.Lifecycle.Events;

public class HostTransitionCompletedEvent : IManagedHostLifecycleEvent
{
    public HostTransitionCompletedEvent(IRuntimeHostState hostState)
    {
        HostState = hostState;
    }

    public IRuntimeHostState HostState { get; }
}
