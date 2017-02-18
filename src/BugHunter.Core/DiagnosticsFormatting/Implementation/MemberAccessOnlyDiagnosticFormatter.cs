using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessOnlyDiagnosticFormatter : MemberAccessDiagnosticFormatterBase, IDiagnosticFormatter
    {
        public Location GetLocation(SyntaxNode expression)
        {
            var memberAccess = GetMemberAccess(expression);

            return memberAccess.Name.GetLocation();
        }

        public string GetDiagnosedUsage(SyntaxNode expression)
        {
            var memberAccess = GetMemberAccess(expression);

            return memberAccess.Name.ToString();
        }
    }
}