using Kantaiko.Hosting.Lifecycle.Events;
using Kantaiko.Routing.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Lifecycle;

public class ApplicationLifecycle : IApplicationLifecycle, IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceProvider _serviceProvider;

    public ApplicationLifecycle(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider serviceProvider)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _serviceProvider = serviceProvider;
    }

    public event AsyncEventHandler<IEventContext<ApplicationStartingEvent>>? ApplicationStarting;
    public event AsyncEventHandler<IEventContext<ApplicationStartedEvent>>? ApplicationStarted;
    public event AsyncEventHandler<IEventContext<ApplicationStoppingEvent>>? ApplicationStopping;
    public event AsyncEventHandler<IEventContext<ApplicationStoppedEvent>>? ApplicationStopped;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);

        return OnApplicationStarting(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStopped.Register(OnApplicationStopped);

        return OnApplicationStopping(cancellationToken);
    }

    private async Task OnApplicationStarting(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = new EventContext<ApplicationStartingEvent>(
            new ApplicationStartingEvent(),
            scope.ServiceProvider,
            cancellationToken
        );

        await ApplicationStarting.InvokeAsync(context);
    }

    private async void OnApplicationStarted()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = new EventContext<ApplicationStartedEvent>(
            new ApplicationStartedEvent(),
            scope.ServiceProvider
        );

        await ApplicationStarted.InvokeAsync(context);
    }

    private async Task OnApplicationStopping(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = new EventContext<ApplicationStoppingEvent>(
            new ApplicationStoppingEvent(),
            scope.ServiceProvider,
            cancellationToken
        );

        await ApplicationStopping.InvokeAsync(context);
    }

    private async void OnApplicationStopped()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = new EventContext<ApplicationStoppedEvent>(
            new ApplicationStoppedEvent(),
            scope.ServiceProvider
        );

        await ApplicationStopped.InvokeAsync(context);
    }
}
