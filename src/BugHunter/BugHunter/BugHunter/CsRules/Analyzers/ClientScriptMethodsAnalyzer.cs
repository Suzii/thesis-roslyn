using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    // TODO should probably look directly for invocation expression instead of member access
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClientScriptMethodsAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CLIENT_SCRIPT_METHODS;

        private static readonly DiagnosticDescriptor Rule = GetRule(DIAGNOSTIC_ID, "ClientScript methods");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            var accessedType = typeof(System.Web.UI.ClientScriptManager);
            var forbiddenMembers = new[]
            {
                nameof(System.Web.UI.ClientScriptManager.RegisterArrayDeclaration),
                nameof(System.Web.UI.ClientScriptManager.RegisterClientScriptBlock),
                nameof(System.Web.UI.ClientScriptManager.RegisterClientScriptInclude),
                nameof(System.Web.UI.ClientScriptManager.RegisterStartupScript)
            };

            RegisterAction(Rule, context, accessedType, forbiddenMembers);
        }
    }
}
