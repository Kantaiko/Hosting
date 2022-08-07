//HintName: Test.ServiceCollectionExtensions.cs
using Kantaiko.Hosting.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Test;

namespace Test;

public static class TestServiceCollectionExtensions
{
    public static IServiceCollection AddTest(this IServiceCollection services)
    {
        services.AddModule<TestModule>();

        return services;
    }
}
