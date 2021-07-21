using Kantaiko.Hosting.Introspection;

namespace Kantaiko.Hosting.Exceptions
{
    public class CircularDependencyException : KantaikoHostingException
    {
        public ModuleInfo First { get; }
        public ModuleInfo Second { get; }

        public CircularDependencyException(ModuleInfo first, ModuleInfo second) : base(
            $"Circular dependency between {first.DisplayName} and {second.DisplayName} was detected")
        {
            First = first;
            Second = second;
        }
    }
}
