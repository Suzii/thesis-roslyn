using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.UI.Page"/> and their access to IsCallback member
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PageIsCallbackAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.PAGE_IS_CALLBACK;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "Page.IsCallback", "RequestHelper.IsCallback()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.Web.UI.Page", "IsCallback");
        }
    }
}
