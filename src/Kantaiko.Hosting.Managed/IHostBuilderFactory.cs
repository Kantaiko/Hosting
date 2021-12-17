using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

public interface IHostBuilderFactory
{
    IHostBuilder CreateHostBuilder();
}
