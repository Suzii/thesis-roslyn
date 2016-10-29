using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usage of <see cref="System.Web.HttpCookie"/> as properties of <see cref="System.Web.HttpRequest"/> or <see cref="System.Web.HttpResponse"/>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestAndResponseCookiesAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_AND_RESPONSE_COOKIE;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.HttpRequestAndResponseCookie_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpRequestAndResponseCookie_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpRequestAndResponseCookie_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, typeof(System.Web.HttpRequest), nameof(System.Web.HttpRequest.Cookies));
            RegisterAction(Rule, context, typeof(System.Web.HttpResponse), nameof(System.Web.HttpResponse.Cookies));
            RegisterAction(Rule, context, typeof(System.Web.HttpRequestBase), nameof(System.Web.HttpRequestBase.Cookies));
            RegisterAction(Rule, context, typeof(System.Web.HttpResponseBase), nameof(System.Web.HttpResponseBase.Cookies));
        }
    }
}
