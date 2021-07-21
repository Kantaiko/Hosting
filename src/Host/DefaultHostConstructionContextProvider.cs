namespace Kantaiko.Hosting.Host
{
    internal class DefaultHostConstructionContextProvider : IHostConstructionContextProvider
    {
        public HostConstructionContext GetHostConstructionContext() => new();
    }
}
