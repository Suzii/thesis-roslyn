using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    /// <summary>
    /// Class containing extension methods for <see cref="IdentifierNameSyntax"/>
    /// </summary>
    public static class IdentifierNameSyntaxExtensions // TODO add unit tests
    {
        /// <summary>
        /// Returns the outer-most parent of "dotted expression" which <param name="identifierNameSyntax"></param> is part of
        /// 
        /// Dotted expression can be either <see cref="MemberAccessExpressionSyntax"/> or <see cref="QualifiedNameSyntax"/>
        /// </summary>
        /// <param name="identifierNameSyntax">Identifier name whose parent should be searched for</param>
        /// <returns>Syntax node that is the highest ancestor of passed invocation expression which is still a "dotted expression"</returns>
        public static SyntaxNode GetOuterMostParentOfDottedExpression(this IdentifierNameSyntax identifierNameSyntax)
        {
            SyntaxNode diagnosedNode = identifierNameSyntax;
            while (diagnosedNode?.Parent != null && (diagnosedNode.Parent.IsKind(SyntaxKind.QualifiedName) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
            {
                diagnosedNode = diagnosedNode.Parent;
            }

            return diagnosedNode;
        }
    }
}