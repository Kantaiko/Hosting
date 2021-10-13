using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kantaiko.Hosting.Hooks;

internal class HookDispatcher : IHookDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HookDispatcher> _logger;
    private readonly HookHandlerCollection _hookHandlerCollection;

    public HookDispatcher(IServiceProvider serviceProvider,
        ILogger<HookDispatcher> logger,
        HookHandlerCollection hookHandlerCollection)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hookHandlerCollection = hookHandlerCollection;
    }

    public void Dispatch<THook>(THook hook) where THook : IHook
    {
        var stopwatch = Stopwatch.StartNew();

        using var _ = _logger.BeginScope(Thread.CurrentThread.ManagedThreadId);

        var hookType = hook.GetType();
        _logger.LogTrace("Dispatching sync hook {HookName}", hookType.Name);

        if (_hookHandlerCollection.HookHandlers.TryGetValue(hookType, out var handlerTypes))
        {
            foreach (var handlerType in handlerTypes)
            {
                _logger.LogTrace("Invoking sync handler {HandlerName} of sync hook {HookName}", handlerType.Name,
                    hookType.Name);

                var handler =
                    (IHookHandler<THook>) ActivatorUtilities.CreateInstance(_serviceProvider, handlerType);
                handler.Handle(hook);
            }
        }

        stopwatch.Stop();
        _logger.LogDebug("Completed sync hook {HookName} in {Elapsed} ms", hookType.Name,
            stopwatch.Elapsed.TotalMilliseconds);
    }

    public async Task DispatchAsync<THook>(THook hook, CancellationToken cancellationToken = default)
        where THook : IAsyncHook
    {
        var stopwatch = Stopwatch.StartNew();

        var hookType = hook.GetType();
        _logger.LogTrace("Dispatching async hook {HookName}", hookType.Name);

        if (_hookHandlerCollection.HookHandlers.TryGetValue(hookType, out var handlerTypes))
        {
            foreach (var handlerType in handlerTypes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var handler = ActivatorUtilities.CreateInstance(_serviceProvider, handlerType);
                switch (handler)
                {
                    case IAsyncHookHandler<THook> asyncHandler:
                        _logger.LogTrace("Invoking async handler {HandlerName} of async hook {HookName}",
                            handlerType.Name, hookType.Name);
                        await asyncHandler.HandleAsync(hook, cancellationToken);
                        continue;
                    case IHookHandler<THook> syncHandler:
                        _logger.LogTrace("Invoking sync handler {HandlerName} of async hook {HookName}",
                            handlerType.Name, hookType.Name);
                        syncHandler.Handle(hook);
                        continue;
                }
            }
        }

        stopwatch.Stop();
        _logger.LogDebug("Completed async hook {HookName} in {Elapsed} ms", hookType.Name,
            stopwatch.Elapsed.TotalMilliseconds);
    }

    public async Task DispatchParallelAsync<THook>(THook hook, CancellationToken cancellationToken = default)
        where THook : IAsyncHook
    {
        var stopwatch = Stopwatch.StartNew();

        var hookType = hook.GetType();
        _logger.LogTrace("Dispatching async hook {HookName} with parallelization", hookType.Name);

        var tasks = new List<Task>();

        if (_hookHandlerCollection.HookHandlers.TryGetValue(hookType, out var handlerTypes))
        {
            foreach (var handlerType in handlerTypes)
            {
                var handler = ActivatorUtilities.CreateInstance(_serviceProvider, handlerType);
                switch (handler)
                {
                    case IAsyncHookHandler<THook> asyncHandler:
                        _logger.LogTrace("Invoking async handler {HandlerName} of async hook {HookName}",
                            handlerType.Name, hookType.Name);
                        tasks.Add(asyncHandler.HandleAsync(hook, cancellationToken));
                        continue;
                    case IHookHandler<THook> syncHandler:
                        _logger.LogTrace("Invoking sync handler {HandlerName} of async hook {HookName}",
                            handlerType.Name, hookType.Name);
                        tasks.Add(Task.Run(() => syncHandler.Handle(hook), cancellationToken));
                        continue;
                }
            }
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();
        _logger.LogDebug("Completed async hook {HookName} in {Elapsed} ms", hookType.Name,
            stopwatch.Elapsed.TotalMilliseconds);
    }
}