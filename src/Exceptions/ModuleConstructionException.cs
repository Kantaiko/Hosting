namespace Kantaiko.Hosting.Exceptions;

public class ModuleConstructionException : KantaikoHostingException
{
    public ModuleConstructionException(Type moduleType, string message) : base(
        $"Unable to construct module of type {moduleType.Name}: {message}") { }
}