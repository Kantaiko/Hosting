using System.Reflection;

namespace Kantaiko.Hosting.Hooks.ApplicationHooks;

public class ApplicationInitializedHook : IHook
{
    public ApplicationInitializedHook(IReadOnlyList<Assembly> assemblies)
    {
        Assemblies = assemblies;
    }

    public IReadOnlyList<Assembly> Assemblies { get; }
}
