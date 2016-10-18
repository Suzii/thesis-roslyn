using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Helpers.CodeFixes
{
    internal class UsingsHelper
    {
        public static CompilationUnitSyntax EnsureUsing(CompilationUnitSyntax root, string namespaceToBeReferenced)
        {
            if (root.Usings.Any(u => u.Name.ToString() == namespaceToBeReferenced))
            {
                return root;
            }

            var namespaceName = SyntaxFactory.ParseName(namespaceToBeReferenced);
            var usingDirective = SyntaxFactory.UsingDirective(namespaceName).NormalizeWhitespace();

            return root.AddUsings(usingDirective);
        }
    }
}