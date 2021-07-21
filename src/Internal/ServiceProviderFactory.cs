using System;
using Kantaiko.Hosting.Hooks;
using Kantaiko.Hosting.Host;
using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Internal
{
    internal class ServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly LoadedHost _loadedHost;
        private readonly IHostModuleHandler? _moduleLoaderHandler;

        public ServiceProviderFactory(LoadedHost loadedHost, IHostModuleHandler? moduleLoaderHandler)
        {
            _loadedHost = loadedHost;
            _moduleLoaderHandler = moduleLoaderHandler;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            services.AddSingleton(_loadedHost.HostInfo);

            foreach (var module in _loadedHost.Modules)
            {
                module.Instance.ConfigureServices(services);
            }

            _moduleLoaderHandler?.ConfigureServices(services, _loadedHost);

            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var serviceProvider = containerBuilder.BuildServiceProvider();

            var hookInitializer = serviceProvider.GetRequiredService<HookInitializer>();
            hookInitializer.Initialize(_loadedHost.HostInfo.Assemblies);

            _moduleLoaderHandler?.Configure(serviceProvider, _loadedHost);

            return serviceProvider;
        }
    }
}
