using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public interface IDiagnosticFormatter
    {
        Location GetLocation(SyntaxNode expression);

        string GetDiagnosedUsage(SyntaxNode expression);
    }
}