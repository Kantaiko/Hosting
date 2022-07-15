//HintName: Test.ServiceCollectionExtensions.cs
using Kantaiko.Hosting.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Test;

namespace Test;

public static class TestServiceCollectionExtensions
{
    public static IServiceCollection AddTest(this IServiceCollection services, Action<TestModuleOptions>? configureDelegate = null)
    {
        services.AddModule<TestModule>();

        services.TryConfigure(configureDelegate);

        return services;
    }
}
