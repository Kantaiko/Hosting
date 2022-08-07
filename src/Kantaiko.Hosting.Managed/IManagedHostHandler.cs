using Kantaiko.Hosting.Managed.Runtime;

namespace Kantaiko.Hosting.Managed;

public interface IManagedHostHandler
{
    Task HandleInitialHostStartAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    Task HandleHostTransitionAsync(IServiceProvider serviceProvider,
        IRuntimeHostState hostState,
        CancellationToken cancellationToken);
}
