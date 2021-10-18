namespace Kantaiko.Hosting.Modules;

public interface IModuleInfoConfigurationMiddleware
{
    void ConfigureInfo(ModuleInfoOptions options);
}