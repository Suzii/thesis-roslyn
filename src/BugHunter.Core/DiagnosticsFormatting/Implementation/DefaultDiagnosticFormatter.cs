using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Default diagnostic formatter class for any <see cref="SyntaxNode"/>
    /// </summary>
    /// <typeparam name="TSyntaxNode">Syntax node or its subtype for which the diagnostic is being raised</typeparam>
    public class DefaultDiagnosticFormatter<TSyntaxNode> : ISyntaxNodeDiagnosticFormatter<TSyntaxNode>
        where TSyntaxNode : SyntaxNode
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <param name="descriptor"></param> based on passed <param name="syntaxNode"></param>.
        ///
        /// MessageFormat will be passed a string version of <param name="syntaxNode"></param>.
        /// Location will be of a whole <param name="syntaxNode"></param>.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="syntaxNode">Syntax node that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given syntax node</returns>
        public virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TSyntaxNode syntaxNode)
            => Diagnostic.Create(descriptor, GetLocation(syntaxNode), GetDiagnosedUsage(syntaxNode));

        /// <summary>
        /// Returns location of passed <param name="syntaxNode"></param>
        /// </summary>
        /// <param name="syntaxNode">Syntax node whose location is being requested</param>
        /// <returns>Loaction of passed syntax node; empty location if syntax node is null</returns>
        protected virtual Location GetLocation(TSyntaxNode syntaxNode)
            => syntaxNode?.GetLocation();

        /// <summary>
        /// Returns string representation of passed <param name="syntaxNode"></param>
        /// </summary>
        /// <param name="syntaxNode">Syntax node whose string representation is being requested</param>
        /// <returns>String representation of passed syntax node; empty string if syntax node is null</returns>
        protected virtual string GetDiagnosedUsage(TSyntaxNode syntaxNode)
            => syntaxNode?.ToString() ?? string.Empty;
    }
}