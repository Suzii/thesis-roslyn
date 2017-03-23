using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    // TODO add more complex logic to get rid of possible follow up member accesses in WhenNotNul that are part of whole ConditionalAccess
    internal class ConditionalAccessDiagnosticFormatter : IDiagnosticFormatter<ConditionalAccessExpressionSyntax>
    {
        public Location GetLocation(ConditionalAccessExpressionSyntax expression)
        {
            return expression.GetLocation();
        }

        public string GetDiagnosedUsage(ConditionalAccessExpressionSyntax expression)
        {
            return expression.ToString();
        }
    }
}