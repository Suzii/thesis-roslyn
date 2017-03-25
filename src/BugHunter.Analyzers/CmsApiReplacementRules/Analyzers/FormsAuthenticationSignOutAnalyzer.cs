using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.Security.FormsAuthentication"/> and their access to SignOut member
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormsAuthenticationSignOutAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.FORMS_AUTHENTICATION_SIGN_OUT;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "FormsAuthentication.SignOut()", "AuthenticationHelper.SignOut()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            ImmutableHashSet.Create("System.Web.Security.FormsAuthentication"),
            ImmutableHashSet.Create("SignOut"));

        private static readonly ApiReplacementForMethodAnalyzer analyzer = new ApiReplacementForMethodAnalyzer(config);

        public override void Initialize(AnalysisContext context)
        {
            analyzer.RegisterAnalyzers(context);
        }
    }
}
