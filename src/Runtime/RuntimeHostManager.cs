using System;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Runtime
{
    internal class RuntimeHostManager : IRuntimeHostManager
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly RuntimeHostState _runtimeHostState;

        public RuntimeHostManager(IHostApplicationLifetime applicationLifetime, RuntimeHostState runtimeHostState)
        {
            _applicationLifetime = applicationLifetime;
            _runtimeHostState = runtimeHostState;
        }

        public void Restart()
        {
            _runtimeHostState.RestartRequested = true;
            _runtimeHostState.RestartRequestedAt = DateTime.Now;

            _applicationLifetime.StopApplication();
        }

        public void Stop()
        {
            _applicationLifetime.StopApplication();
        }
    }
}