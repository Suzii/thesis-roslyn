using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestBrowserAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_BROWSER;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Request.Browser", "BrowserHelper.GetBrowser()");
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig apiReplacementConfig = new ApiReplacementConfig(Rule,
            ImmutableHashSet.Create("System.Web.HttpBrowserCapabilities", "System.Web.HttpBrowserCapabilitiesBase"),
            ImmutableHashSet.Create("Browser"));

        private static readonly ApiReplacementForMemberAnalyzer apiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(apiReplacementConfig);

        public override void Initialize(AnalysisContext context)
        {
            apiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
