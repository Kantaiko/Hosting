namespace Kantaiko.Hosting.Managed;

public interface ISharedServiceProvider : IServiceProvider
{
    void SetService(Type serviceType, object instance);

    object GetRequiredService(Type serviceType, Func<IServiceProvider, object> factory);
}
