using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    /// <summary>
    /// Helper class containing extensions for <see cref="ClassDeclarationSyntax"/>
    /// </summary>
    public static class ClassDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Determines whether <paramref name="classDeclarationSyntax" /> is declared as abstract
        /// </summary>
        /// <param name="classDeclarationSyntax">Class declaration to be inspected</param>
        /// <returns>True if class is abstract</returns>
        public static bool IsAbstract(this ClassDeclarationSyntax classDeclarationSyntax)
            => classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword));

        /// <summary>
        /// Determines whether <paramref name="classDeclarationSyntax" /> is declared as partial
        /// </summary>
        /// <param name="classDeclarationSyntax">Class declaration to be inspected</param>
        /// <returns>True if class is partial</returns>
        public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax)
            => classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));

        /// <summary>
        /// Determines whether <paramref name="classDeclarationSyntax" /> is declared as public
        /// </summary>
        /// <param name="classDeclarationSyntax">Class declaration to be inspected</param>
        /// <returns>True if class is public</returns>
        public static bool IsPublic(this ClassDeclarationSyntax classDeclarationSyntax)
            => classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));

        /// <summary>
        /// Adds <paramref name="newBaseTypeName" /> as a base class of <paramref name="classDeclaration" />
        /// </summary>
        /// <param name="classDeclaration">Class declaration to be modified</param>
        /// <param name="newBaseTypeName">Modified class declaration with new base type</param>
        /// <returns>Modified class with base class specified</returns>
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
            => classDeclaration.BaseList?.Types ?? default(SeparatedSyntaxList<BaseTypeSyntax>);
    }
}