using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClientScriptMethodsAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CLIENT_SCRIPT_METHODS;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "ClientScript methods");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            ImmutableHashSet.Create("System.Web.UI.ClientScriptManager"), 
            ImmutableHashSet.Create("RegisterArrayDeclaration", "RegisterClientScriptBlock", "RegisterClientScriptInclude", "RegisterStartupScript"));

        private static readonly ApiReplacementForMethodAnalyzer analyzer = new ApiReplacementForMethodAnalyzer(config);

        public override void Initialize(AnalysisContext context)
        {
            analyzer.RegisterAnalyzers(context);
        }
    }
}
