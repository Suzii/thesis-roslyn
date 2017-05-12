using System.Collections.Immutable;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.MethodInvocationHeuristics
{
    /// <summary>
    /// ApiReplacementAnalyzerForMethod WITHOUT syntax heuristic optimization in MethodInvocationAnalyzer
    /// 
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    //[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class FormsAuthenticationSignOutAnalyzer_NoOptimization : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
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
