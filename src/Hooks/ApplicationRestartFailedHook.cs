using Kantaiko.Hosting.Runtime;

namespace Kantaiko.Hosting.Hooks;

public class ApplicationRestartFailedHook : IAsyncHook
{
    public ApplicationRestartFailedHook(IRuntimeHostState hostState)
    {
        HostState = hostState;
    }

    public IRuntimeHostState HostState { get; }
}