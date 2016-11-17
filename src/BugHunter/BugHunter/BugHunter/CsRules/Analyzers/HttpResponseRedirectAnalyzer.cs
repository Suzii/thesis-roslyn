using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpResponseRedirectAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_RESPONSE_REDIRECT;

        private static readonly DiagnosticDescriptor Rule = GetRule(DIAGNOSTIC_ID, "Response.Redirect");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.Web.HttpResponse", "Redirect");
            RegisterAction(Rule, context, "System.Web.HttpResponseBase", "Redirect");
        }
    }
}
