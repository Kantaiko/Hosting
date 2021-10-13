namespace Kantaiko.Hosting.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleNameAttribute : Attribute, IModuleInfoConfigurationMiddleware
{
    private readonly string _name;

    public ModuleNameAttribute(string name)
    {
        _name = name;
    }

    public void ConfigureInfo(ModuleInfoOptions options)
    {
        options.Name = _name;
    }
}
