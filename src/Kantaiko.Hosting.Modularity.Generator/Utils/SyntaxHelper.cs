using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kantaiko.Hosting.Modularity.Generator.Utils;

internal static class SyntaxHelper
{
    public static string? ExtractTypeName(TypeSyntax typeSyntax)
    {
        return typeSyntax switch
        {
            IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.Text,
            QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax.Right.Identifier.Text,
            _ => null
        };
    }
}
