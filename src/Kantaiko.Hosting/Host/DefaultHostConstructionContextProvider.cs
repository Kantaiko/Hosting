namespace Kantaiko.Hosting.Host;

internal class DefaultHostConstructionContextProvider : IHostConstructionContextProvider
{
    public HostConstructionContext GetHostConstructionContext() => new();

    private static DefaultHostConstructionContextProvider? _instance;

    public static DefaultHostConstructionContextProvider Instance =>
        _instance ??= new DefaultHostConstructionContextProvider();
}
