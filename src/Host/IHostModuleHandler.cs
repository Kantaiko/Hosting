using System;
using Kantaiko.Hosting.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace Kantaiko.Hosting.Host
{
    public interface IHostModuleHandler
    {
        void ConfigureServices(IServiceCollection services, LoadedHost host) { }
        void Configure(IServiceProvider provider, LoadedHost host) { }
    }
}
