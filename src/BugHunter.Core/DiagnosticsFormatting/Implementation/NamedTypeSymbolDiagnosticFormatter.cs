using System.Linq;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class NamedTypeSymbolDiagnosticFormatter : ISymbolDiagnosticFormatter<INamedTypeSymbol>
    {
        public Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, INamedTypeSymbol namedTypeSymbol)
        {
            // TODO
            var location = namedTypeSymbol.Locations.FirstOrDefault(l => l.SourceTree.FilePath.EndsWith("x.cs"));
            var diagnostic = Diagnostic.Create(descriptor, location, namedTypeSymbol.Name);

            return diagnostic;
        }
    }
}