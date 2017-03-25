using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpResponseRedirectAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_RESPONSE_REDIRECT;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Response.Redirect()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            new []{ "System.Web.HttpResponse", "System.Web.HttpResponseBase"},
            new []{ "Redirect"});

        private static readonly ApiReplacementForMethodAnalyzer analyzer = new ApiReplacementForMethodAnalyzer(config);

        public override void Initialize(AnalysisContext context)
        {
            analyzer.RegisterAnalyzers(context);
        }
    }
}
