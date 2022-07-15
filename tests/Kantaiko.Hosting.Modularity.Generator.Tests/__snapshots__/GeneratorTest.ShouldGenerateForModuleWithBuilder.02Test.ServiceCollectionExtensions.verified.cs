//HintName: Test.ServiceCollectionExtensions.cs
using Kantaiko.Hosting.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Test;

namespace Test;

public static class TestServiceCollectionExtensions
{
    public static IServiceCollection AddTest(this IServiceCollection services, Action<TestModuleBuilder>? configureDelegate = null)
    {
        services.AddModule<TestModule>();

        configureDelegate?.Invoke(new TestModuleBuilder(services));

        return services;
    }
}
