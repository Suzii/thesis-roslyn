using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
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
        /// <summary>
        /// The ID for diagnostics raises by <see cref="FormsAuthenticationSignOutAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.FormsAuthenticationSignOut;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DiagnosticId, "FormsAuthentication.SignOut()", "AuthenticationHelper.SignOut()");

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new []{ "System.Web.Security.FormsAuthentication" },
            new []{ "SignOut" });

        private static readonly ApiReplacementForMethodAnalyzer Analyzer = new ApiReplacementForMethodAnalyzer(Config);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            Analyzer.RegisterAnalyzers(context);
        }
    }
}
