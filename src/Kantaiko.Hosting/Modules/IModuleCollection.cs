namespace Kantaiko.Hosting.Modules;

public interface IModuleCollection
{
    public void Add<T>() where T : class, IModule;
}