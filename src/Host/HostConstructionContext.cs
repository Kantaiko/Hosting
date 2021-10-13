namespace Kantaiko.Hosting.Host;

public class HostConstructionContext
{
    public HostConstructionContext(IReadOnlyList<Type>? moduleTypes = null)
    {
        ModuleTypes = moduleTypes ?? ArraySegment<Type>.Empty;
    }

    public IReadOnlyList<Type> ModuleTypes { get; }
}