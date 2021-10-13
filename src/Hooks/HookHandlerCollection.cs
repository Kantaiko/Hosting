using Microsoft.Collections.Extensions;

namespace Kantaiko.Hosting.Hooks;

internal class HookHandlerCollection
{
    public MultiValueDictionary<Type, Type> HookHandlers { get; } = new();
}