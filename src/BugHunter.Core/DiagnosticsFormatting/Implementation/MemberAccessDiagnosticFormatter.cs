using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessDiagnosticFormatter : IDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        public Location GetLocation(MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess?.GetLocation();
        }

        public string GetDiagnosedUsage(MemberAccessExpressionSyntax memberAccess)
        {
            return $"{memberAccess?.Expression}.{memberAccess?.Name}";
        }
    }
}