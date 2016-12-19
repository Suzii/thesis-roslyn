using System.Collections.Immutable;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HttpRequestQueryStringAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.HTTP_REQUEST_QUERY_STRING;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "QueryString[]", "QueryHelper.Get<Type>()");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        
        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.Web.HttpRequest", "QueryString");
            RegisterAction(Rule, context, "System.Web.HttpRequestBase", "QueryString");
        }
    }
}
