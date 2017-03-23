using System.Collections.Immutable;
using BugHunter.Core.Helpers.DiagnosticDescriptionBuilders;
using BugHunter.Core._experiment;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private readonly IAccessAnalyzer _memberAccessAnalyzer = new SimpleMemberAccessAnalyzer(Rule, "System.Web.UI.Page", "IsCallback");
        private readonly ConditionalAccessAnalyzer _conditionalAccessAnalyzer = new ConditionalAccessAnalyzer(Rule, "System.Web.UI.Page", "IsCallback");

        // TODO: Pull this whole method to BaseApiReplacementForMem?? should be same for all api replacement member analyzers
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // TODO consider registering compilation action first, get the INamedTypeSymbol and pass to underlying analyzers
            context.RegisterSyntaxNodeAction(_memberAccessAnalyzer.Run, SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(_conditionalAccessAnalyzer.Run, SyntaxKind.ConditionalAccessExpression);
        }
    }
}
