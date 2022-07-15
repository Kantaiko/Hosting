using Kantaiko.Hosting.Modularity.Introspection;
using Microsoft.Extensions.Hosting;

namespace Kantaiko.Hosting.Modularity.TypeRegistration;

internal class TypeRegistrationManager : IHostedService
{
    private readonly HostInfo _hostInfo;
    private readonly IEnumerable<ITypeRegistrationHandler> _registrationHandlers;

    public TypeRegistrationManager(HostInfo hostInfo, IEnumerable<ITypeRegistrationHandler> registrationHandlers)
    {
        _hostInfo = hostInfo;
        _registrationHandlers = registrationHandlers;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var registrationHandlers = new List<ITypeRegistrationHandler>();

        foreach (var assembly in _hostInfo.Assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                foreach (var registrationHandler in _registrationHandlers)
                {
                    registrationHandlers.Add(registrationHandler);

                    var handled = registrationHandler.Handle(type);
                    if (handled) break;
                }
            }
        }

        foreach (var registrationHandler in registrationHandlers)
        {
            registrationHandler.Complete();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
