using System.Collections.Immutable;
using System.Threading;
using BugHunter.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers
{
    /// <summary>
    /// This class serves serves as benchmarking for SyntaxNode - MemberAccess - 2 heavy callbacks (half task of SingelCallbackAnalyzer)
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MemberAccessTwoCallbacksAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.SYNTAX_NODE_MEMBER_ACCESS_SINGLE_CALLBACK;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "Serves as benchmarking for SyntaxNode - MemberAccess - 2 heavy callbacks",
                messageFormat: "Serves as benchmarking for SyntaxNode - MemberAccess - 2 heavy callbacks",
                category: nameof(AnalyzerCategories.BenchmarkingBaselines),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction((c) => Thread.Sleep(50), SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction((c) => Thread.Sleep(50), SyntaxKind.ClassDeclaration);
        }
    }
}
