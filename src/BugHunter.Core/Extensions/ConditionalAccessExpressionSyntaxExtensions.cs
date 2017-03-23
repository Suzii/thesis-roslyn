using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    public static class ConditionalAccessExpressionSyntaxExtensions
    {
        public static MemberBindingExpressionSyntax GetFirstMemberBindingExpression(
            this ConditionalAccessExpressionSyntax conditionalAccessExpression)
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