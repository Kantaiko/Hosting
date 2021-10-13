namespace Kantaiko.Hosting.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleVersionAttribute : Attribute, IModuleInfoConfigurationMiddleware
{
    private readonly Version _version;

    public ModuleVersionAttribute(string version)
    {
        _version = Version.Parse(version);
    }

    public void ConfigureInfo(ModuleInfoOptions options)
    {
        options.Version = _version;
    }
}
