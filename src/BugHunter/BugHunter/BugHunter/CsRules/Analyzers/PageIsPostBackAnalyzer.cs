using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.UI.Page"/> and their access to IsPostBack member
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageIsPostBackAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.PAGE_IS_POST_BACK;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Page.IsPostBack", "RequestHelper.IsPostBack()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.Web.UI.Page", "IsPostBack");
        }
    }
}
