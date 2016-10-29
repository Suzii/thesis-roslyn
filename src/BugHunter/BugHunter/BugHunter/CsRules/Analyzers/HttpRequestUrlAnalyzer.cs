using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestUrlAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_URL;

        private static readonly DiagnosticDescriptor Rule = GetRule(DIAGNOSTIC_ID, "Request.Url", "RequestContext.Url");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, typeof(System.Web.HttpRequest), nameof(System.Web.HttpRequest.Url));
            RegisterAction(Rule, context, typeof(System.Web.HttpRequestBase), nameof(System.Web.HttpRequestBase.Url));
        }
    }
}
