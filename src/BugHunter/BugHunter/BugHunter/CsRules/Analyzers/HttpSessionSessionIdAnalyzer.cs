using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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
    public class HttpSessionSessionIdAnalyzer : BaseMemberAccessAnalyzer
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
            RegisterAction(Rule, context, typeof(System.Web.SessionState.HttpSessionState), nameof(System.Web.SessionState.HttpSessionState.SessionID));
            RegisterAction(Rule, context, typeof(System.Web.HttpSessionStateBase), nameof(System.Web.HttpSessionStateBase.SessionID));
        }
    }
}
