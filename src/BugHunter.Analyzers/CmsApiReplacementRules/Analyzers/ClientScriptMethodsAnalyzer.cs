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

        private readonly IAccessAnalyzer _arrayDeclarationAnalyzer = new MemberInvocationAnalyzer(Rule, "System.Web.UI.ClientScriptManager", "RegisterArrayDeclaration");
        private readonly IAccessAnalyzer _scriptBlockAnalyzer = new MemberInvocationAnalyzer(Rule, "System.Web.UI.ClientScriptManager", "RegisterClientScriptBlock");
        private readonly IAccessAnalyzer _scriptIncludeAnalyzer = new MemberInvocationAnalyzer(Rule, "System.Web.UI.ClientScriptManager", "RegisterClientScriptInclude");
        private readonly IAccessAnalyzer _startupScriptAnalyzer = new MemberInvocationAnalyzer(Rule, "System.Web.UI.ClientScriptManager", "RegisterStartupScript");

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(syntaxNodeContext =>
            {
                _arrayDeclarationAnalyzer.Run(syntaxNodeContext);
                _scriptBlockAnalyzer.Run(syntaxNodeContext);
                _scriptIncludeAnalyzer.Run(syntaxNodeContext);
                _startupScriptAnalyzer.Run(syntaxNodeContext);
            }, SyntaxKind.InvocationExpression);

            //var accessedType = "System.Web.UI.ClientScriptManager";

            //RegisterAction(Rule, context, accessedType, 
            //    "RegisterArrayDeclaration",
            //    "RegisterClientScriptBlock",
            //    "RegisterClientScriptInclude",
            //    "RegisterStartupScript");
        }
    }
}
