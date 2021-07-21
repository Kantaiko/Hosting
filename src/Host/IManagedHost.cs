﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Kantaiko.Hosting.Exceptions;
using Kantaiko.Hosting.Runtime;

namespace Kantaiko.Hosting.Host
{
    /// <summary>
    /// Represents the managed restartable host.
    /// </summary>
    public interface IManagedHost : IDisposable
    {
        /// <summary>
        /// Represents the current state of the host.
        /// </summary>
        ManagedHostState State { get; }

        /// <summary>
        /// Indicates that the host is started.
        /// </summary>
        [MemberNotNullWhen(true, nameof(Services))]
        bool IsStarted { get; }

        /// <summary>
        /// Starts the host.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidHostStateException">throws if host is already started</exception>
        /// <returns></returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the host.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidHostStateException">throws if host is not started</exception>
        /// <returns></returns>
        Task StopAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Restarts the host.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>current runtime host state</returns>
        Task<IRuntimeHostState> RestartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Waits for the host shutdown.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidHostStateException">throws if host is not started</exception>
        /// <returns></returns>
        Task WaitForShutdownAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Waits for the host restart.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>current runtime host state</returns>
        Task<IRuntimeHostState> WaitForRestartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Service provider for currently running host instance.
        /// Null if the host is not started.
        /// </summary>
        IServiceProvider? Services { get; }
    }
}
