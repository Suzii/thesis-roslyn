// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="syntaxNode" />.
        ///
        /// MessageFormat will be passed a string version of <paramref name="syntaxNode" />.
        /// Location will be of a whole <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="syntaxNode">Syntax node that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given syntax node</returns>
        public virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TSyntaxNode syntaxNode)
            => Diagnostic.Create(descriptor, GetLocation(syntaxNode), GetDiagnosedUsage(syntaxNode));

        /// <summary>
        /// Returns location of passed <paramref name="syntaxNode" />
        /// </summary>
        /// <param name="syntaxNode">Syntax node whose location is being requested</param>
        /// <returns>Loaction of passed syntax node; empty location if syntax node is null</returns>
        protected virtual Location GetLocation(TSyntaxNode syntaxNode)
            => syntaxNode?.GetLocation();

        /// <summary>
        /// Returns string representation of passed <paramref name="syntaxNode" />
        /// </summary>
        /// <param name="syntaxNode">Syntax node whose string representation is being requested</param>
        /// <returns>String representation of passed syntax node; empty string if syntax node is null</returns>
        protected virtual string GetDiagnosedUsage(TSyntaxNode syntaxNode)
            => syntaxNode?.ToString() ?? string.Empty;
    }
}