using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticFormatting.Implementation
{
    internal class MemberAccessDiagnosticFormatter : MemberAccessDiagnosticFormatterBase, IDiagnosticFormatter
    {
        public Location GetLocation(ExpressionSyntax expression)
        {
            var memberAccess = GetMemberAccess(expression);

            return memberAccess.GetLocation();
        }

        public string GetDiagnosedUsage(ExpressionSyntax expression)
        {
            var memberAccess = GetMemberAccess(expression);

            return $"{memberAccess.Expression}.{memberAccess.Name}";
        }
    }
}