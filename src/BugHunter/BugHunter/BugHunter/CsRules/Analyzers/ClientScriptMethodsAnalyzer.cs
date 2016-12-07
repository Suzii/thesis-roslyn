using System.Collections.Immutable;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClientScriptMethodsAnalyzer : BaseMemberInvocatoinAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CLIENT_SCRIPT_METHODS;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "ClientScript methods");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            var accessedType = typeof(System.Web.UI.ClientScriptManager);

            RegisterAction(Rule, context, accessedType, 
                nameof(System.Web.UI.ClientScriptManager.RegisterArrayDeclaration),
                nameof(System.Web.UI.ClientScriptManager.RegisterClientScriptBlock),
                nameof(System.Web.UI.ClientScriptManager.RegisterClientScriptInclude),
                nameof(System.Web.UI.ClientScriptManager.RegisterStartupScript));
        }
    }
}
