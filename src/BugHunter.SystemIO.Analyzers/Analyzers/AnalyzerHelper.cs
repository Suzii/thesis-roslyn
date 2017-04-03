using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.SystemIO.Analyzers.Analyzers
{
    public static class AnalyzerHelper
    {
        public static readonly string[] WhiteListedTypeNames =
        {
            "System.IO.IOException",
            "System.IO.Stream"
        };

        public static DiagnosticDescriptor GetRule(string diagnosticId) => new DiagnosticDescriptor(diagnosticId,
                title: new LocalizableResourceString(nameof(Resources.SystemIo_Title), Resources.ResourceManager, typeof(Resources)),
                messageFormat: new LocalizableResourceString(nameof(Resources.SystemIo_MessageFormat), Resources.ResourceManager, typeof(Resources)),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.SystemIo_Description), Resources.ResourceManager, typeof(Resources)));

        private static readonly ISyntaxNodeDiagnosticFormatter<SyntaxNode> DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();

        public static Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, IdentifierNameSyntax identifierName)
        {
            var rootOfDottedExpression = GetOuterMostParentOfDottedExpression(identifierName);
            var diagnosedNode = rootOfDottedExpression.Parent.IsKind(SyntaxKind.ObjectCreationExpression)
                ? rootOfDottedExpression.Parent
                : rootOfDottedExpression;

            return DiagnosticFormatter.CreateDiagnostic(rule, diagnosedNode);
        }

        // TODO add unit tests
        private static SyntaxNode GetOuterMostParentOfDottedExpression(IdentifierNameSyntax identifierNameSyntax)
        {
            SyntaxNode diagnosedNode = identifierNameSyntax;
            while (diagnosedNode?.Parent != null && (diagnosedNode.Parent.IsKind(SyntaxKind.QualifiedName) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.InvocationExpression)))
            {
                diagnosedNode = diagnosedNode.Parent;
            }

            return diagnosedNode;
        }
    }
}