using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class ConditionalAccessDiagnosticFormatter : MemberAccessDiagnosticFormatterBase, IDiagnosticFormatter
    {
        public Location GetLocation(SyntaxNode expression)
        {
            //var conditionalAccess = (ConditionalAccessExpressionSyntax) expression;

            //return conditionalAccess.GetLocation();
            return expression.GetLocation();
        }

        public string GetDiagnosedUsage(SyntaxNode expression)
        {
            //var memberAccess = GetMemberAccess(expression);

            //return $"{memberAccess.Expression}.{memberAccess.Name}";
            return expression.ToString();
        }
    }
}