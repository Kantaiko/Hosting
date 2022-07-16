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

    public event AsyncEventHandler<IAsyncEventContext<ApplicationStartingEvent>>? ApplicationStarting;
    public event SyncEventHandler<IEventContext<ApplicationStartedEvent>>? ApplicationStarted;

    public event AsyncEventHandler<IAsyncEventContext<ApplicationStoppingEvent>>? ApplicationStopping;
    public event SyncEventHandler<IEventContext<ApplicationStoppedEvent>>? ApplicationStopped;

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

        var context = new AsyncEventContext<ApplicationStartingEvent>(
            new ApplicationStartingEvent(),
            scope.ServiceProvider,
            cancellationToken
        );

        await ApplicationStarting.InvokeAsync(context);
    }

    private void OnApplicationStarted()
    {
        using var scope = _serviceProvider.CreateScope();

        var context = new EventContext<ApplicationStartedEvent>(
            new ApplicationStartedEvent(),
            scope.ServiceProvider
        );

        ApplicationStarted?.Invoke(context);
    }

    private async Task OnApplicationStopping(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = new AsyncEventContext<ApplicationStoppingEvent>(
            new ApplicationStoppingEvent(),
            scope.ServiceProvider,
            cancellationToken
        );

        await ApplicationStopping.InvokeAsync(context);
    }

    private void OnApplicationStopped()
    {
        using var scope = _serviceProvider.CreateScope();

        var context = new EventContext<ApplicationStoppedEvent>(
            new ApplicationStoppedEvent(),
            scope.ServiceProvider
        );

        ApplicationStopped?.Invoke(context);
    }
}
