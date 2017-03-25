using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    public class ApiReplacementForMemberAnalyzer
    {
        private readonly ISyntaxNodeAnalyzer _simpleMemberAccessAnalyzer;
        private readonly ISyntaxNodeAnalyzer _conditionalAccessAnalyzer;

        public ApiReplacementForMemberAnalyzer(ApiReplacementConfig config)
        {
            _simpleMemberAccessAnalyzer = new SimpleMemberAccessAnalyzer(config, DiagnosticFormatterFactory.CreateMemberAccessFormatter());
            _conditionalAccessAnalyzer = new ConditionalAccessAnalyzer(config, DiagnosticFormatterFactory.CreateConditionalAccessFormatter());
        }

        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            // TODO consider registering compilation action first, get the INamedTypeSymbol and pass to underlying analyzers
            analysisContext.RegisterSyntaxNodeAction(_simpleMemberAccessAnalyzer.Run, SyntaxKind.SimpleMemberAccessExpression);
            analysisContext.RegisterSyntaxNodeAction(_conditionalAccessAnalyzer.Run, SyntaxKind.ConditionalAccessExpression);
        }
    }
}