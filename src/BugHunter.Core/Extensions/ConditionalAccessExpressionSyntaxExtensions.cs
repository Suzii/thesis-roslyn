using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    /// <summary>
    /// Helper class containing extensions for <see cref="ConditionalAccessExpressionSyntax"/>
    /// </summary>
    public static class ConditionalAccessExpressionSyntaxExtensions
    {
        /// <summary>
        /// Returns first <see cref="MemberBindingExpressionSyntax"/> of <paramref name="conditionalAccessExpression" />
        /// </summary>
        /// <param name="conditionalAccessExpression">Conditional access to be inspected</param>
        /// <returns>First member binding expression</returns>
        public static MemberBindingExpressionSyntax GetFirstMemberBindingExpression(this ConditionalAccessExpressionSyntax conditionalAccessExpression)
        {
            var firstMemberBindingExpressionOnTheRightOfTheDot = conditionalAccessExpression
                ?.WhenNotNull
                ?.DescendantNodesAndSelf()
                .OfType<MemberBindingExpressionSyntax>()
                .FirstOrDefault();

            return firstMemberBindingExpressionOnTheRightOfTheDot;
        }
    }
}