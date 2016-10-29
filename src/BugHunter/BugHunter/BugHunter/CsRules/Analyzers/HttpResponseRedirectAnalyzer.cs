using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpResponseRedirectAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_RESPONSE_REDIRECT;
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, 
            title: new LocalizableResourceString(nameof(CsResources.HttpResponseRedirect_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpResponseRedirect_MessageFormat), CsResources.ResourceManager, typeof(CsResources)), 
            category: AnalyzerCategories.CS_RULES, 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpResponseRedirect_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, typeof(System.Web.HttpResponse), nameof(System.Web.HttpResponse.Redirect));
            RegisterAction(Rule, context, typeof(System.Web.HttpResponseBase), nameof(System.Web.HttpResponse.Redirect));
        }
    }
}
