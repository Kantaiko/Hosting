namespace Kantaiko.Hosting.Host
{
    public static class ManagedHostExtensions
    {
        public static void Run(this IManagedHost host)
        {
            host.Start();
            host.WaitForShutdown();
        }

        public static void Start(this IManagedHost host)
        {
            host.StartAsync().GetAwaiter().GetResult();
        }

        public static void Stop(this IManagedHost host)
        {
            host.StopAsync().GetAwaiter().GetResult();
        }

        public static void Restart(this IManagedHost host)
        {
            host.RestartAsync().GetAwaiter().GetResult();
        }

        public static void WaitForShutdown(this IManagedHost host)
        {
            host.WaitForShutdownAsync().GetAwaiter().GetResult();
        }

        public static void WaitForRestart(this IManagedHost host)
        {
            host.WaitForRestartAsync().GetAwaiter().GetResult();
        }
    }
}
