using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    public class ApiReplacementForMethodAnalyzer
    {
        private readonly ISyntaxNodeAnalyzer _methodInvocationAnalyzer;

        public ApiReplacementForMethodAnalyzer(ApiReplacementConfig config)
        {
            _methodInvocationAnalyzer = new MethodInvocationAnalyzer(config, DiagnosticFormatterFactory.CreateMemberInvocationFormatter());
        }

        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            // TODO consider registering compilation action first, get the INamedTypeSymbol and pass to underlying analyzers
            analysisContext.RegisterSyntaxNodeAction(_methodInvocationAnalyzer.Run, SyntaxKind.InvocationExpression);
        }
    }
}