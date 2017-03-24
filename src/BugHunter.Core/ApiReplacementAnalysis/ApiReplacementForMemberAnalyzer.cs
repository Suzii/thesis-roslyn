using BugHunter.Core.Analyzers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    public class ApiReplacementForMemberAnalyzer
    {
        private readonly ApiReplacementConfig _config;
        private readonly bool _locationOnlyOnMember;
        // TODO: add possibility to configure from the outside
        private ISyntaxNodeAnalyzer _simpleMemberAccessAnalyzer;
        private ISyntaxNodeAnalyzer _conditionalAccessAnalyzer;

        public ApiReplacementForMemberAnalyzer(ApiReplacementConfig config, bool locationOnlyOnMember = false)
        {
            _config = config;
            _locationOnlyOnMember = locationOnlyOnMember;

            _simpleMemberAccessAnalyzer = new SimpleMemberAccessAnalyzer(config);
            _conditionalAccessAnalyzer = new ConditionalAccessAnalyzer(config);
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