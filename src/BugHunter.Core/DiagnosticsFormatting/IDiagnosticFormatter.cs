using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    /// <summary>
    /// Generic interface for diagnostic formatters
    /// </summary>
    /// <typeparam name="TNodeOrSymbol"></typeparam>
    public interface IDiagnosticFormatter<in TNodeOrSymbol>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <param name="descriptor"></param> based on passed <param name="nodeOrSymbol"></param>.
        /// 
        /// Arguments for MessageFormat diagnostic's location depend on specific implementations
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="nodeOrSymbol">Syntax node or symbol that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given node or symbol</returns>
        Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TNodeOrSymbol nodeOrSymbol);
    }
}