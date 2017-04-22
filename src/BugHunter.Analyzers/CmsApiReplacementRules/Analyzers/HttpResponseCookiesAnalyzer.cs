using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usage of <see cref="System.Web.HttpCookie"/> as properties of <see cref="System.Web.HttpResponse"/>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpResponseCookiesAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_RESPONSE_COOKIES;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID, "Response.Cookies", "CookieHelper.ResponseCookies");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig apiReplacementConfig = new ApiReplacementConfig(Rule,
            new []{ "System.Web.HttpResponse", "System.Web.HttpResponseBase"},
            new []{ "Cookies"});

        private static readonly ApiReplacementForMemberAnalyzer apiReplacementAnalyzer = new ApiReplacementForMemberAnalyzer(apiReplacementConfig);

        public override void Initialize(AnalysisContext context)
        {
            apiReplacementAnalyzer.RegisterAnalyzers(context);
        }
    }
}
