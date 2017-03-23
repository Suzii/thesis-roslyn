using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public interface IDiagnosticFormatter<in TSyntaxKind>
        where TSyntaxKind : SyntaxNode
    {
        Location GetLocation(TSyntaxKind expression);

        string GetDiagnosedUsage(TSyntaxKind expression);
    }
}