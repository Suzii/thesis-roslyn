using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticFormatting.Implementation
{
    internal class MemberInvocationOnlyDiagnosticFormatter : MemberInvocationDiagnosticFormatterBase, IDiagnosticFormatter
    {
        /// <summary>
        /// Returns location of method invocation only. 
        /// 
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed,
        /// location of 'WhereLike("col", "val")' is returned.
        /// Throws if <param name="expression"></param> cannot be casted to <see cref="MemberAccessExpressionSyntax"/>
        /// </summary>
        /// <param name="expression">Invocation expression</param>
        /// <returns>Location of nested method invocation</returns>
        public Location GetLocation(ExpressionSyntax expression)
        {
            var invocationExpression = GetInvocationExpression(expression);
            var memberAccess = GetUnderlyingMemberAccess(invocationExpression);

            var statLocation = memberAccess.Name.GetLocation().SourceSpan.Start;
            var endLocation = invocationExpression.GetLocation().SourceSpan.End;
            var location = Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(statLocation, endLocation));

            return location;
        }

        public string GetDiagnosedUsage(ExpressionSyntax expression)
        {
            var invocationExpression = GetInvocationExpression(expression);
            var memberAccess = GetUnderlyingMemberAccess(invocationExpression);

            return $"{memberAccess.Name}{invocationExpression.ArgumentList}";
        }
    }
}