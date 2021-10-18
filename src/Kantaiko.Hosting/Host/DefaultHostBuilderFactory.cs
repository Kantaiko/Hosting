using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public class DefaultHostBuilderFactory : IHostBuilderFactory
{
    public IHostBuilder CreateHostBuilder(HostConstructionContext constructionContext)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(constructionContext.Args);
    }

    private static DefaultHostBuilderFactory? _instance;
    public static DefaultHostBuilderFactory Instance => _instance ??= new DefaultHostBuilderFactory();
}
