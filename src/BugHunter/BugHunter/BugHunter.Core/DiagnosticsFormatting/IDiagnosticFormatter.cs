using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public interface IDiagnosticFormatter
    {
        Location GetLocation(ExpressionSyntax expression);

        string GetDiagnosedUsage(ExpressionSyntax expression);
    }
}