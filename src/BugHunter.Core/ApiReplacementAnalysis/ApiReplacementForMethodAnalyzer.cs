using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    /// <summary>
    /// Strategy to be used for all method API Replacements analyzers
    ///
    /// It register the analysis action based on passed <see cref="ApiReplacementConfig"/> object
    /// and makes sure analysis is run for all method invocations
    /// </summary>
    public class ApiReplacementForMethodAnalyzer
    {
        private readonly ISyntaxNodeAnalyzer _methodInvocationAnalyzer;

        /// <summary>
        /// Constructor accepting <param name="config"></param> object to be used during the analysis
        /// </summary>
        /// <param name="config">Configuration to be used for the analysis, specifying forbidden method names on specific types and diagnostic descriptor</param>
        public ApiReplacementForMethodAnalyzer(ApiReplacementConfig config)
        {
            _methodInvocationAnalyzer = new MethodInvocationAnalyzer(config, new MethodInvocationDiagnosticFormatter());
        }

        /// <summary>
        /// Registers the analyzers on <param name="analysisContext"></param>
        ///
        /// Makes sure the analysis is run for all method invocations
        /// and enables concurrent analysis execution and disables analysis of generated code
        /// </summary>
        /// <param name="analysisContext">Contex of the analysis to register the action on</param>
        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            analysisContext.RegisterSyntaxNodeAction(_methodInvocationAnalyzer.Run, SyntaxKind.InvocationExpression);
        }
    }
}