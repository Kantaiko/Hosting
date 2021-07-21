using System.Threading;
using System.Threading.Tasks;

namespace Kantaiko.Hosting.Hooks
{
    public interface IHookDispatcher
    {
        void Dispatch<THook>(THook hook) where THook : IHook;

        Task DispatchAsync<THook>(THook hook, CancellationToken cancellationToken = default) where THook : IAsyncHook;
        Task DispatchParallelAsync<THook>(THook hook, CancellationToken cancellationToken = default) where THook : IAsyncHook;
    }
}