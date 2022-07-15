using Microsoft.CodeAnalysis;

namespace Kantaiko.Hosting.Modularity.Generator.Utils;

internal static class NameHelper
{
    public static string ExtractModuleName(ITypeSymbol moduleType)
    {
        return moduleType.Name.Replace("Module", "");
    }
}
