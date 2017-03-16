using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestUrlAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_URL;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Request.Url", "RequestContext.URL");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();    

            RegisterAction(Rule, context, "System.Web.HttpRequest", "Url");
            RegisterAction(Rule, context, "System.Web.HttpRequestBase", "Url");
        }
    }
}
