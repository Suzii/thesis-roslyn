using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    /// <summary>
    /// Generic interface for diagnostic formatters
    /// </summary>
    /// <typeparam name="TNodeOrSymbol"></typeparam>
    public interface IDiagnosticFormatter<in TNodeOrSymbol>
    {
        Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TNodeOrSymbol nodeOrSymbol);
    }
}