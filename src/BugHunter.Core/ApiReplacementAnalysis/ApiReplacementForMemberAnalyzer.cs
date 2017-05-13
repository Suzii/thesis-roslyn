using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    /// <summary>
    /// Strategy to be used for all member API Replacements analyzers
    ///
    /// It register the analysis action based on passed <see cref="ApiReplacementConfig"/> object
    /// and makes sure analysis is run on all possible member accesses (both simple and conditional)
    /// </summary>
    public class ApiReplacementForMemberAnalyzer
    {
        private readonly ISyntaxNodeAnalyzer _simpleMemberAccessAnalyzer;
        private readonly ISyntaxNodeAnalyzer _conditionalAccessAnalyzer;

        /// <summary>
        /// Constructor accepting <param name="config"></param> object to be used during the analysis
        /// </summary>
        /// <param name="config">Configuration to be used for the analysis, specifying forbidden members on specific types and diagnostic descriptor</param>
        public ApiReplacementForMemberAnalyzer(ApiReplacementConfig config)
        {
            _simpleMemberAccessAnalyzer = new SimpleMemberAccessAnalyzer(config, new MemberAccessDiagnosticFormatter());
            _conditionalAccessAnalyzer = new ConditionalAccessAnalyzer(config, new ConditionalAccessDiagnosticFormatter());
        }

        /// <summary>
        /// Registers the analyzers on <param name="analysisContext"></param>
        ///
        /// Makes sure the analysis is run on all possible member accesses (both simple and conditional)
        /// and enables concurrent analysis execution and disables analysis of generated code
        /// </summary>
        /// <param name="analysisContext">Contex of the analysis to register the action on</param>
        public void RegisterAnalyzers(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            analysisContext.EnableConcurrentExecution();

            analysisContext.RegisterSyntaxNodeAction(_simpleMemberAccessAnalyzer.Run, SyntaxKind.SimpleMemberAccessExpression);
            analysisContext.RegisterSyntaxNodeAction(_conditionalAccessAnalyzer.Run, SyntaxKind.ConditionalAccessExpression);
        }
    }
}