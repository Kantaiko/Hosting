using Kantaiko.Hosting.Runtime;

namespace Kantaiko.Hosting.Hooks.HostHooks;

public class ManagedHostRestartFailedHook : IAsyncHook
{
    public ManagedHostRestartFailedHook(IRuntimeHostState hostState)
    {
        HostState = hostState;
    }

    public IRuntimeHostState HostState { get; }
}
