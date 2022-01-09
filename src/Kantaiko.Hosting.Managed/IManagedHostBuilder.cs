using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Managed;

public interface IManagedHostBuilder
{
    IManagedHostBuilder UseHostBuilderFactory(IHostBuilderFactory hostBuilderFactory);
    IManagedHostBuilder UseManagedHostHandler(IManagedHostHandler managedHostHandler);

    IManagedHostBuilder ConfigureHostBuilder(Action<IHostBuilder> configureDelegate);

    IManagedHost Build();
}
