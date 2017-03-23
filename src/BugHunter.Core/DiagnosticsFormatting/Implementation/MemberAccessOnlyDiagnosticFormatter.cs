using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessOnlyDiagnosticFormatter : IDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        public Location GetLocation(MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess?.Name?.GetLocation();
        }

        public string GetDiagnosedUsage(MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess?.Name?.Identifier.ValueText;
        }
    }
}