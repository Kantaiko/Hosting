using System.Runtime.CompilerServices;

namespace Kantaiko.Hosting.Modularity.Generator.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
