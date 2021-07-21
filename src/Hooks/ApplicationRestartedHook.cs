using Kantaiko.Hosting.Runtime;

namespace Kantaiko.Hosting.Hooks
{
    public class ApplicationRestartedHook : IAsyncHook
    {
        public ApplicationRestartedHook(IRuntimeHostState hostState)
        {
            HostState = hostState;
        }

        public IRuntimeHostState HostState { get; }
    }
}
