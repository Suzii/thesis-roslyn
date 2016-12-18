using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticFormatting.Implementation
{
    internal class MemberInvocationDiagnosticFormatter : MemberInvocationDiagnosticFormatterBase, IDiagnosticFormatter
    {
        public Location GetLocation(ExpressionSyntax expression)
        {
            return expression.GetLocation();
        }

        public string GetDiagnosedUsage(ExpressionSyntax expression)
        {
            //var memberAccess = GetUnderlyingMemberAccess(GetInvocationExpression(expression));

            return $"{expression}";
        }
    }
}