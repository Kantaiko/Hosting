namespace Kantaiko.Hosting.Modularity.Exceptions;

public class ModuleConstructionException : Exception
{
    public ModuleConstructionException(Type moduleType, string message) : base(
        $"Unable to construct module of type \"{moduleType.Name}\": {message}") { }
}
