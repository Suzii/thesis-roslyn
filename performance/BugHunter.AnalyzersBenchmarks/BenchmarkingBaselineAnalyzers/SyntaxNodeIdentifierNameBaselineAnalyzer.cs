using System.Collections.Immutable;
using BugHunter.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers
{
    /// <summary>
    /// This class serves as a baseline for all analyzers that have callbacks to SyntaxNodeAction of type <c>SyntaxKind.IdentifierName</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeIdentifierNameBaselineAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.SYNTAX_NODE_IDENTIFIER_NAME;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "Serves as benchmarking baseline for SyntaxNode - IdentifierName",
                messageFormat: "Serves as benchmarking baseline for SyntaxNode - IdentifierName",
                category: nameof(AnalyzerCategories.BenchmarkingBaselines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            // Empty callback to serve as a baseline for benchmarks
        }
    }
}
