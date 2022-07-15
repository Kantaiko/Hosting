using System.Text;
using Microsoft.CodeAnalysis;

namespace Kantaiko.Hosting.Modularity.Generator.Utils;

internal class NamespaceImportEmitter
{
    private readonly SortedSet<string> _namespaces = new();
    private readonly Dictionary<string, ITypeSymbol> _typeNames = new();

    public void AddNamespace(string ns)
    {
        _namespaces.Add(ns);
    }

    public string GetTypeExpression(ITypeSymbol symbol)
    {
        var fullName = GetFullNestedTypeName(symbol);

        if (_typeNames.TryGetValue(fullName, out var anotherSymbol) &&
            !SymbolEqualityComparer.Default.Equals(symbol, anotherSymbol))
        {
            return symbol.ContainingNamespace + "." + fullName;
        }

        _typeNames[fullName] = symbol;
        _namespaces.Add(symbol.ContainingNamespace.ToString());

        return fullName;
    }

    public void Emit(ref StringBuilder builder)
    {
        var namespaceCapacity = _namespaces.Sum(ns => ns.Length + 8);

        var newBuilder = new StringBuilder(namespaceCapacity + builder.Capacity + 1);

        foreach (var ns in _namespaces)
        {
            newBuilder.AppendLine($"using {ns};");
        }

        newBuilder.AppendLine();

        newBuilder.Append(builder, 0, builder.Length);

        builder = newBuilder;
    }

    public void Clear()
    {
        _namespaces.Clear();
        _typeNames.Clear();
    }

    private static string GetFullNestedTypeName(ITypeSymbol symbol)
    {
        var fullName = symbol.Name;

        while (symbol.ContainingType is not null)
        {
            symbol = symbol.ContainingType;
            fullName = symbol.Name + "." + fullName;
        }

        return fullName;
    }
}
