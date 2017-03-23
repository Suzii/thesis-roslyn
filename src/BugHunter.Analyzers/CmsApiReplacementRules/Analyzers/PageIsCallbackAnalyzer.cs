using System.Collections.Immutable;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using BugHunter.Core._experiment;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.UI.Page"/> and their access to IsCallback member
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageIsCallbackAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.PAGE_IS_CALLBACK;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Page.IsCallback", "RequestHelper.IsCallback()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule, 
            ImmutableHashSet.Create("System.Web.UI.Page"),
            ImmutableHashSet.Create("IsCallback"));
        
        private static readonly ApiReplacementForMemberAnalyzer ApiReplacementForMemberAnalyzer = new ApiReplacementForMemberAnalyzer(Config);

        public override void Initialize(AnalysisContext context)
        {
            ApiReplacementForMemberAnalyzer.RegisterAnalyzers(context);
        }
    }
}
