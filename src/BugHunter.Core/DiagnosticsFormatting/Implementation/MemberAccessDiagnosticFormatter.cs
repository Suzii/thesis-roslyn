using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessDiagnosticFormatter : MemberAccessDiagnosticFormatterBase, IDiagnosticFormatter
    {
        public Location GetLocation(SyntaxNode expression)
        {
            var memberAccess = GetMemberAccess(expression);

            return memberAccess.GetLocation();
        }

        public string GetDiagnosedUsage(SyntaxNode expression)
        {
            var memberAccess = GetMemberAccess(expression);

            return $"{memberAccess.Expression}.{memberAccess.Name}";
        }
    }
}