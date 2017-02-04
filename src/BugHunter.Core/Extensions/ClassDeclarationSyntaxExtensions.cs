using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    public static class ClassDeclarationSyntaxExtensions
    {
        public static bool IsAbstract(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword));
        }

        public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
        }

        public static bool IsPublic(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));
        }
    }
}