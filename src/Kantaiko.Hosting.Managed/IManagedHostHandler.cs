using Kantaiko.Hosting.Managed.Runtime;

namespace Kantaiko.Hosting.Managed;

public interface IManagedHostHandler
{
    Task HandleInitialHostStart(IServiceProvider serviceProvider, CancellationToken cancellationToken);

    Task HandleHostTransition(IServiceProvider serviceProvider,
        IRuntimeHostState hostState,
        CancellationToken cancellationToken);
}
