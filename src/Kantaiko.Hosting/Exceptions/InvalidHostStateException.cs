using Kantaiko.Hosting.Host;

namespace Kantaiko.Hosting.Exceptions;

public class InvalidHostStateException : KantaikoHostingException
{
    public InvalidHostStateException(IReadOnlyList<ManagedHostState> expectedStates, ManagedHostState actual)
        : base($"Host is in invalid state. Expected: {RenderExpectedStates(expectedStates)}, actual: {actual}.") { }

    private static string RenderExpectedStates(IReadOnlyList<ManagedHostState> expectedStates)
    {
        if (expectedStates.Count == 1)
        {
            return expectedStates[0].ToString();
        }

        return string.Join(", ", expectedStates.SkipLast(1)) + " or " + expectedStates[^1];
    }
}