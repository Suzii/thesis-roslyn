using System;
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

        public static ClassDeclarationSyntax WithBaseClass(this ClassDeclarationSyntax classDeclaration, string newBaseTypeName)
        {
            var simpleBaseTypeSyntax = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseName(newBaseTypeName));

            var originalBaseList = GetOriginalBaseList(classDeclaration);
            if (HasSomeBaseType(classDeclaration))
            {
                originalBaseList = originalBaseList.RemoveAt(0);
            }

            var newBaseList = originalBaseList.Insert(0, simpleBaseTypeSyntax);
            var newClassDeclaration = classDeclaration.WithBaseList(SyntaxFactory.BaseList(newBaseList).NormalizeWhitespace().WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));
            newClassDeclaration =
                newClassDeclaration.WithIdentifier(newClassDeclaration.Identifier.NormalizeWhitespace().WithTrailingTrivia(SyntaxFactory.Space));

            return newClassDeclaration;
        }

        private static bool HasSomeBaseType(ClassDeclarationSyntax classDeclaration)
        {
            var firstTypeInBaseList = classDeclaration.BaseList?.Types.FirstOrDefault()?.Type;
            if (firstTypeInBaseList == null)
            {
                return false;
            }

            return !IsInterfaceName(firstTypeInBaseList);
        }

        /// <summary>
        /// Heuristically identifies if typeSyntax is an interface
        /// </summary>
        /// <param name="typeSyntax">Type syntax to be analyzed</param>
        /// <returns>True if TypeSyntax looks like interface name</returns>
        private static bool IsInterfaceName(TypeSyntax typeSyntax)
        {
            var fullTypeName = typeSyntax.ToString();
            // zero if does not contain dot
            var typeName = fullTypeName.Substring(fullTypeName.LastIndexOf(".", StringComparison.Ordinal) + 1);
            return typeName.StartsWith("I") && typeName.Length > 1 && char.IsUpper(typeName[1]);
        }

        private static SeparatedSyntaxList<BaseTypeSyntax> GetOriginalBaseList(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.BaseList?.Types ?? new SeparatedSyntaxList<BaseTypeSyntax>();
        }
    }
}