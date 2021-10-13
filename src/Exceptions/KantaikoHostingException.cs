namespace Kantaiko.Hosting.Exceptions;

public abstract class KantaikoHostingException : Exception
{
    public KantaikoHostingException(string message) : base(message) { }
}