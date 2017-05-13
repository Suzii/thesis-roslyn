// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting
{
    /// <summary>
    /// Generic interface for diagnostic formatters accepting any <see cref="ISymbol"/>
    /// </summary>
    /// <typeparam name="TSymbol">Diagnosed symbol to be used for diagnostic</typeparam>
    public interface ISymbolDiagnosticFormatter<in TSymbol> : IDiagnosticFormatter<TSymbol>
        where TSymbol : ISymbol
    {
    }
}