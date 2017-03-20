using System.Collections.Immutable;
using BugHunter.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers
{
    /// <summary>
    /// This class serves serves as benchmarking for SyntaxNode - MemberAccess - with an empty callback
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MemberAccessEmptyCallbacksAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.SYNTAX_NODE_MEMBER_ACCESS_SINGLE_CALLBACK;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "Serves as benchmarking for SyntaxNode - MemberAccess - empty callback",
                messageFormat: "Serves as benchmarking for SyntaxNode - MemberAccess - empty callback",
                category: nameof(AnalyzerCategories.BenchmarkingBaselines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction((c) => { }, SyntaxKind.ClassDeclaration);
        }
    }
}
