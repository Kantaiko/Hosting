using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;
using Microsoft.Extensions.DependencyInjection;
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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // ReSharper disable once AsyncVoidLambda
        _applicationLifetime.ApplicationStarted.Register(async () =>
        {
            using var scope = _serviceProvider.CreateScope();

            var context = new EventContext<ApplicationStartedEvent>(
                new ApplicationStartedEvent(), scope.ServiceProvider);

            await _applicationLifecycle.ApplicationStarted.Handle(context);
        });

        // ReSharper disable once AsyncVoidLambda
        _applicationLifetime.ApplicationStopped.Register(async () =>
        {
            using var scope = _serviceProvider.CreateScope();

            var context = new EventContext<ApplicationStoppedEvent>(
                new ApplicationStoppedEvent(), scope.ServiceProvider);

            await _applicationLifecycle.ApplicationStopped.Handle(context);
        });

        using var scope = _serviceProvider.CreateScope();

        var context = new EventContext<ApplicationStartingEvent>(new ApplicationStartingEvent(),
            scope.ServiceProvider, cancellationToken: cancellationToken);

        await _applicationLifecycle.ApplicationStarting.Handle(context);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = new EventContext<ApplicationStoppingEvent>(new ApplicationStoppingEvent(),
            scope.ServiceProvider, cancellationToken: cancellationToken);

        await _applicationLifecycle.ApplicationStopping.Handle(context);
    }
}
