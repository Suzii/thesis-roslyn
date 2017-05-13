using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.AnalyzersVersions.SystemIO.Helpers
{
    /// <summary>
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    internal class SystemIoDiagnosticFormatter : ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax>
    {
        /// <inheritdoc/>
        public Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, IdentifierNameSyntax identifierName)
        {
            var rootOfDottedExpression = identifierName.GetOuterMostParentOfDottedExpression();
            var diagnosedNode = rootOfDottedExpression.Parent.IsKind(SyntaxKind.ObjectCreationExpression) || rootOfDottedExpression.Parent.IsKind(SyntaxKind.InvocationExpression)
                ? rootOfDottedExpression.Parent
                : rootOfDottedExpression;

            return Diagnostic.Create(descriptor, GetLocation(diagnosedNode), GetDiagnosedUsage(diagnosedNode));
        }

        private Location GetLocation(SyntaxNode syntaxNode)
            => syntaxNode?.GetLocation();

        private string GetDiagnosedUsage(SyntaxNode syntaxNode)
            => syntaxNode?.ToString() ?? string.Empty;
    }
}