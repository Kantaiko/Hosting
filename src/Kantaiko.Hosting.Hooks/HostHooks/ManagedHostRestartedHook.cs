using Kantaiko.Hosting.Runtime;

namespace Kantaiko.Hosting.Hooks.HostHooks;

public class ManagedHostRestartedHook : IAsyncHook
{
    public ManagedHostRestartedHook(IRuntimeHostState hostState)
    {
        HostState = hostState;
    }

    public IRuntimeHostState HostState { get; }
}
