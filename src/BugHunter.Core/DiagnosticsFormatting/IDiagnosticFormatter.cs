// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    /// <summary>
    /// Generic interface for diagnostic formatters
    /// </summary>
    /// <typeparam name="TNodeOrSymbol">Syntax node or ISymbol</typeparam>
    public interface IDiagnosticFormatter<in TNodeOrSymbol>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="nodeOrSymbol" />.
        ///
        /// Arguments for MessageFormat diagnostic's location depend on specific implementations
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="nodeOrSymbol">Syntax node or symbol that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given node or symbol</returns>
        Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TNodeOrSymbol nodeOrSymbol);
    }
}