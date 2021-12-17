namespace Kantaiko.Hosting.Managed;

public static class ManagedHostExtensions
{
    public static void Run(this IManagedHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.Start();
        host.WaitForShutdown();
    }

    public static async Task RunAsync(this IManagedHost host, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(host);

        await host.StartAsync(cancellationToken);
        await host.WaitForShutdownAsync(cancellationToken);
    }

    public static void Start(this IManagedHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.StartAsync().GetAwaiter().GetResult();
    }

    public static void Stop(this IManagedHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.StopAsync().GetAwaiter().GetResult();
    }

    public static void Restart(this IManagedHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.RestartAsync().GetAwaiter().GetResult();
    }

    public static void WaitForShutdown(this IManagedHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.WaitForShutdownAsync().GetAwaiter().GetResult();
    }

    public static void WaitForRestart(this IManagedHost host)
    {
        ArgumentNullException.ThrowIfNull(host);

        host.WaitForRestartAsync().GetAwaiter().GetResult();
    }
}
