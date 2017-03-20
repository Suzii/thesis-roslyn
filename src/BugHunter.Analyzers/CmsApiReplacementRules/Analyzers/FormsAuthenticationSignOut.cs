using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.Security.FormsAuthentication"/> and their access to SignOut member
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormsAuthenticationSignOut : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.FORMS_AUTHENTICATION_SIGN_OUT;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "FormsAuthentication.SignOut()", "AuthenticationHelper.SignOut()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            RegisterAction(Rule, context, "System.Web.Security.FormsAuthentication", "SignOut");
        }
    }
}
