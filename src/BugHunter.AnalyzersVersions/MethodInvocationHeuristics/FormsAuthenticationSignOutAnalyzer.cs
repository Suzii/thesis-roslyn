using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.MethodInvocationHeuristics
{

    /// <summary>
    /// ApiReplacementAnalyzerForMethod WITH syntax heuristic optimization in MethodInvocationAnalyzer
    /// 
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormsAuthenticationSignOutAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "BH1012";

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRulesProvider.GetRule(DIAGNOSTIC_ID, "FormsAuthentication.SignOut()", "AuthenticationHelper.SignOut()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            new []{ "System.Web.Security.FormsAuthentication" },
            new []{ "SignOut" });

        private static readonly ApiReplacementForMethodAnalyzer analyzer = new ApiReplacementForMethodAnalyzer(config);

        public override void Initialize(AnalysisContext context)
        {
            analyzer.RegisterAnalyzers(context);
        }
    }
}
