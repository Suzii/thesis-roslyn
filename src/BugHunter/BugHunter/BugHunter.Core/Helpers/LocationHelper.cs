using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.Helpers
{
    public static class LocationHelper
    {

        /// <summary>
        /// Returns location of method invocation only. 
        /// 
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed,
        /// location of 'WhereLike("col", "val")' is returned.
        /// Throws if <param name="invocationExpression"></param> cannot be casted to <see cref="MemberAccessExpressionSyntax"/>
        /// </summary>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>Location of nested method invocation</returns>
        public static Location GetLocationOfMethodInvocationOnly(InvocationExpressionSyntax invocationExpression)
        {
            var memberAccess = MethodInvocationHelper.GetUnderlyingMemberAccess(invocationExpression);

            var statLocation = memberAccess.Name.GetLocation().SourceSpan.Start;
            var endLocation = invocationExpression.GetLocation().SourceSpan.End;
            var location = Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(statLocation, endLocation));

            return location;
        }

        /// <summary>
        /// Returns location of whole invocation.
        /// 
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed,
        /// location of 'condition.Or().WhereLike("col", "val")' is returned.
        /// </summary>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>Location of whole invocation expression</returns>
        public static Location GetLocationOfWholeInvocation(InvocationExpressionSyntax invocationExpression)
        {
            var location = invocationExpression.GetLocation();

            return location;
        }
    }
}