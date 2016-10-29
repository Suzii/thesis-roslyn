using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestUserHostAddressAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_USER_HOST_ADDRESS;
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, 
            title: new LocalizableResourceString(nameof(CsResources.HttpRequestUserHostAddress_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpRequestUserHostAddress_MessageFormat), CsResources.ResourceManager, typeof(CsResources)), 
            category: AnalyzerCategories.CS_RULES, 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpRequestUserHostAddress_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, typeof(System.Web.HttpRequest), nameof(System.Web.HttpRequest.UserHostAddress));
            RegisterAction(Rule, context, typeof(System.Web.HttpRequestBase), nameof(System.Web.HttpRequestBase.UserHostAddress));
        }
    }
}
