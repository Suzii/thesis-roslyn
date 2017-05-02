using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
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
            _simpleMemberAccessAnalyzer = new SimpleMemberAccessAnalyzer(config, new MemberAccessDiagnosticFormatter());
            _conditionalAccessAnalyzer = new ConditionalAccessAnalyzer(config, new ConditionalAccessDiagnosticFormatter());
        }

        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();
            
            analysisContext.RegisterSyntaxNodeAction(_simpleMemberAccessAnalyzer.Run, SyntaxKind.SimpleMemberAccessExpression);
            analysisContext.RegisterSyntaxNodeAction(_conditionalAccessAnalyzer.Run, SyntaxKind.ConditionalAccessExpression);
        }
    }
}