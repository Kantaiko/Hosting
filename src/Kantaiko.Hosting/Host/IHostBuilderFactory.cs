using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Host;

public interface IHostBuilderFactory
{
    IHostBuilder CreateHostBuilder(HostConstructionContext constructionContext);
}
