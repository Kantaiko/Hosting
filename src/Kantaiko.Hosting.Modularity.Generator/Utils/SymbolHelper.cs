using Microsoft.CodeAnalysis;

namespace Kantaiko.Hosting.Modularity.Generator.Utils;

internal static class SymbolHelper
{
    public static bool IsImplementsInterface(INamedTypeSymbol typeSymbol, string fullTypeName)
    {
        return typeSymbol.AllInterfaces.Any(x => x.ToString() == fullTypeName);
    }
}
