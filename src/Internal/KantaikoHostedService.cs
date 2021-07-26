using System.Threading;
using System.Threading.Tasks;
using Kantaiko.Hosting.Hooks;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Internal
{
    internal class KantaikoHostedService : IHostedService
    {
        private readonly IHookDispatcher _hookDispatcher;
        private readonly IHostApplicationLifetime _lifetime;

        public KantaikoHostedService(IHookDispatcher hookDispatcher, IHostApplicationLifetime lifetime)
        {
            _hookDispatcher = hookDispatcher;
            _lifetime = lifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var applicationStartupHook = new ApplicationStartupHook();
            await _hookDispatcher.DispatchAsync(applicationStartupHook, cancellationToken);

            _lifetime.ApplicationStarted.Register(() =>
            {
                var applicationReadyHook = new ApplicationReadyHook();
                _hookDispatcher.DispatchAsync(applicationReadyHook, cancellationToken);
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var applicationShutdownHook = new ApplicationShutdownHook();
            await _hookDispatcher.DispatchAsync(applicationShutdownHook, cancellationToken);
        }
    }
}
