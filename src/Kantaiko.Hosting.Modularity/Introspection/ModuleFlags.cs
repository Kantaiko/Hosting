namespace Kantaiko.Hosting.Modularity.Introspection;

[Flags]
public enum ModuleFlags
{
    None = 0,
    Library = 1 << 0,
    Hidden = 1 << 1
}
