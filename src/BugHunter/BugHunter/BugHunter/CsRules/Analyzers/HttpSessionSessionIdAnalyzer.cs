using System.Collections.Immutable;
using System.Web;
using BugHunter.Helpers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.Web.HttpSessionStateBase"/> or <see cref="System.Web.SessionState.HttpSessionState"/> and their access to SessionID member
    /// </summary>
    /// <remarks>
    /// Note that both classes need to be checked for access as they do not derive from one another and both can be used. For different scenarios check code in test file.
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpSessionSessionIdAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HttpSessionSessionId;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.HttpSessionSessionId_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.HttpSessionSessionId_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CsRules,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.HttpSessionSessionId_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // TODO register two separate actions or modify MemberAccessAnalyzer to accept multiple types???
            var memberName = nameof(System.Web.HttpSessionStateBase.SessionID);
            var analyzerForHttpSessionStateBase = new MemberAccessAnalyzer(Rule, typeof(System.Web.HttpSessionStateBase), memberName);
            var analyzerForHttpSessionState = new MemberAccessAnalyzer(Rule, typeof(System.Web.SessionState.HttpSessionState), memberName);

            context.RegisterSyntaxNodeAction(c => analyzerForHttpSessionStateBase.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
            context.RegisterSyntaxNodeAction(c => analyzerForHttpSessionState.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
        }
    }
}
