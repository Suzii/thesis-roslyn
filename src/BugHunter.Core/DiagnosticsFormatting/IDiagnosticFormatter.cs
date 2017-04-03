using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public interface IDiagnosticFormatter<in TNodeOrSymbol>
    {
        Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TNodeOrSymbol nodeOrSymbol);
    }

    public interface ISyntaxNodeDiagnosticFormatter<in TSyntaxNode> : IDiagnosticFormatter<TSyntaxNode>
        where TSyntaxNode : SyntaxNode
    {
    }

    public interface ISymbolDiagnosticFormatter : IDiagnosticFormatter<ISymbol>
    {
    }
}