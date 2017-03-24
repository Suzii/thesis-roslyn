using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using BugHunter.Core._experiment;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ClientScriptMethodsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.CLIENT_SCRIPT_METHODS;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "ClientScript methods");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            ImmutableHashSet.Create("System.Web.UI.ClientScriptManager"),
            ImmutableHashSet.Create("RegisterArrayDeclaration", "RegisterClientScriptBlock", "RegisterClientScriptInclude", "RegisterStartupScript"));

        private static readonly IAccessAnalyzer _memeberInvocationAnalyzer = new MemberInvocationAnalyzer(Config);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(_memeberInvocationAnalyzer.Run, SyntaxKind.InvocationExpression);

            //var accessedType = "System.Web.UI.ClientScriptManager";

            //RegisterAction(Rule, context, accessedType, 
            //    "RegisterArrayDeclaration",
            //    "RegisterClientScriptBlock",
            //    "RegisterClientScriptInclude",
            //    "RegisterStartupScript");
        }
    }
}
