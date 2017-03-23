using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class DefaultDiagnosticFormatter : IDiagnosticFormatter<SyntaxNode>
    {
        public Location GetLocation(SyntaxNode expression)
        {
            return expression.GetLocation();
        }

        public string GetDiagnosedUsage(SyntaxNode expression)
        {
            return expression.ToString();
        }
    }
}