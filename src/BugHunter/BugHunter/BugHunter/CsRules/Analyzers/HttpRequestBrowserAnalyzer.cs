using System.Collections.Immutable;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestBrowserAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_URL;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Request.Browser", "BrowserHelper.GetBrowser()");
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.Web.HttpBrowserCapabilities", "Browser");
            RegisterAction(Rule, context, "System.Web.HttpBrowserCapabilitiesBase", "Browser");
        }
    }
}
