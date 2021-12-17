namespace Kantaiko.Hosting.Managed;

public class SingleHostBuilderFactoryProvider : IHostBuilderFactoryProvider
{
    private readonly IHostBuilderFactory _hostBuilderFactory;

    public SingleHostBuilderFactoryProvider(IHostBuilderFactory hostBuilderFactory)
    {
        _hostBuilderFactory = hostBuilderFactory;
    }

    public bool Volatile => false;

    public IHostBuilderFactory GetHostBuilderFactory()
    {
        return _hostBuilderFactory;
    }
}
