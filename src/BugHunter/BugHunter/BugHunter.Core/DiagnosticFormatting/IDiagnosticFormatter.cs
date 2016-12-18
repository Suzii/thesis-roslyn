using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticFormatting
{
    public interface IDiagnosticFormatter
    {
        Location GetLocation(ExpressionSyntax expression);

        string GetDiagnosedUsage(ExpressionSyntax expression);
    }
}