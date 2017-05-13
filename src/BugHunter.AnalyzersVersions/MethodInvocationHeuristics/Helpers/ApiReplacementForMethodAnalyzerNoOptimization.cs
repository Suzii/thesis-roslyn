using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.MethodInvocationHeuristics.Helpers
{
    /// <summary>
    /// This is duplicate version of <see cref="ApiReplacementForMethodAnalyzer"/> BUT withou syntax heuristics optimization.
    ///
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    public class ApiReplacementForMethodAnalyzerNoOptimization
    {
        private readonly ISyntaxNodeAnalyzer _methodInvocationAnalyzer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiReplacementForMethodAnalyzerNoOptimization"/> class.
        /// </summary>
        /// <param name="config">Configuration of the analysis</param>
        public ApiReplacementForMethodAnalyzerNoOptimization(ApiReplacementConfig config)
        {
            _methodInvocationAnalyzer = new MethodInvocationAnalyzerNoOptimization(config, new MethodInvocationDiagnosticFormatter());
        }

        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            analysisContext.RegisterSyntaxNodeAction(_methodInvocationAnalyzer.Run, SyntaxKind.InvocationExpression);
        }
    }
}