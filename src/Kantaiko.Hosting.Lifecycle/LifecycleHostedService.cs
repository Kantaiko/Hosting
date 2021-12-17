using Kantaiko.Hosting.Lifecycle.Events;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Lifecycle;

internal class LifecycleHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IApplicationLifecycle _applicationLifecycle;
    private readonly IServiceProvider _serviceProvider;

    public LifecycleHostedService(IHostApplicationLifetime applicationLifetime,
        IApplicationLifecycle applicationLifecycle, IServiceProvider serviceProvider)
    {
        _applicationLifetime = applicationLifetime;
        _applicationLifecycle = applicationLifecycle;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _applicationLifetime.ApplicationStarted.Register(() =>
        {
            var context = new LifecycleEventContext<ApplicationStartedEvent>(new ApplicationStartedEvent(),
                _serviceProvider, CancellationToken.None);

            _applicationLifecycle.ApplicationStarted.Handle(context);
        });

        _applicationLifetime.ApplicationStopped.Register(() =>
        {
            var context = new LifecycleEventContext<ApplicationStoppedEvent>(new ApplicationStoppedEvent(),
                _serviceProvider, CancellationToken.None);

            _applicationLifecycle.ApplicationStopped.Handle(context);
        });

        var context = new LifecycleEventContext<ApplicationStartingEvent>(new ApplicationStartingEvent(),
            _serviceProvider, cancellationToken);

        return _applicationLifecycle.ApplicationStarting.Handle(context);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        var context = new LifecycleEventContext<ApplicationStoppingEvent>(new ApplicationStoppingEvent(),
            _serviceProvider, cancellationToken);

        return _applicationLifecycle.ApplicationStopping.Handle(context);
    }
}
