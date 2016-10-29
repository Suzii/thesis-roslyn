using System.Linq;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    public class UsingsHelper
    {
       /// <summary>
       /// Generates using directive for all <param name="usings"/> that are not already present in usings of <param name="root" />
       /// </summary>
       /// <param name="root">Document to add usings to</param>
       /// <param name="usings">Usings to be added</param>
       /// <returns></returns>
        public static CompilationUnitSyntax EnsureUsings(CompilationUnitSyntax root, params string[] usings)
        {
            if (usings == null || usings.Length == 0)
            {
                return root;
            }

            var currentUsings = root.Usings.Select(u => u.Name.ToString());
            var toBeAddedUsings = usings.Except(currentUsings).ToList();
            if (toBeAddedUsings.IsNullOrEmpty())
            {
                return root;
            }

            var usingDirectives = toBeAddedUsings.Select(usingName => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingName))).ToArray();
            var newCompilationUnitSyntax = root.AddUsings(usingDirectives);

            return newCompilationUnitSyntax;
        }
    }
}