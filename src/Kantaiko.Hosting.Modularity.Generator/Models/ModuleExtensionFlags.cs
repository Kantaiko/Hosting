namespace Kantaiko.Hosting.Modularity.Generator.Models;

[Flags]
public enum ModuleExtensionFlags
{
    None,
    WithOptions = 1 << 0,
    WithBuilder = 1 << 1,
    Both = WithOptions | WithBuilder,
}
