namespace Kantaiko.Hosting.Managed.Exceptions;

public class UnrecoverableHostStateException : Exception
{
    public UnrecoverableHostStateException(Exception exception) : base(
        "The managed host has been terminated because it failed to shutdown/restart and could not be recovered",
        exception) { }
}
