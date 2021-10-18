namespace Kantaiko.Hosting.Host;

public class HostConstructionContext
{
    public HostConstructionContext(string[]? args = null, IReadOnlyList<Type>? moduleTypes = null)
    {
        Args = args;
        ModuleTypes = moduleTypes ?? ArraySegment<Type>.Empty;
    }

    public string[]? Args { get; }
    public IReadOnlyList<Type> ModuleTypes { get; }
}
