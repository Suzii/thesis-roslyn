using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    public class DefaultDiagnosticFormatter<TSyntaxNode> : ISyntaxNodeDiagnosticFormatter<TSyntaxNode>
        where TSyntaxNode: SyntaxNode
    {
        public virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TSyntaxNode syntaxNode)
            => Diagnostic.Create(descriptor, GetLocation(syntaxNode), GetDiagnosedUsage(syntaxNode));

        public virtual Location GetLocation(TSyntaxNode syntaxNode)
            => syntaxNode?.GetLocation();

        public virtual string GetDiagnosedUsage(TSyntaxNode syntaxNode)
            => syntaxNode?.ToString() ?? string.Empty;
    }
}