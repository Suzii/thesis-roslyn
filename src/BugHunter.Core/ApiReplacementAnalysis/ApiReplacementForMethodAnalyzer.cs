using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    public class ApiReplacementForMethodAnalyzer
    {
        // TODO: add possibility to configure from the outside
        private readonly ISyntaxNodeAnalyzer _memberInvocationAnalyzer;

        public ApiReplacementForMethodAnalyzer(ApiReplacementConfig config)
        {
            _memberInvocationAnalyzer = new MemberInvocationAnalyzer(config, DiagnosticFormatterFactory.CreateMemberInvocationFormatter());
        }

        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            // TODO consider registering compilation action first, get the INamedTypeSymbol and pass to underlying analyzers
            analysisContext.RegisterSyntaxNodeAction(_memberInvocationAnalyzer.Run, SyntaxKind.InvocationExpression);
        }
    }
}