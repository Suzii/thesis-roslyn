using System.Collections.Immutable;
using BugHunter.Core.Helpers.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereLikeMethodAnalyzer : BaseMemberAccessAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.WHERE_LIKE_METHOD;

        private static readonly DiagnosticDescriptor Rule = GetRule(DIAGNOSTIC_ID, "WhereLike() or WhereNotLike() methods");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            var accessedType = "CMS.DataEngine.WhereConditionBase`1";
            var forbiddenMembers = new[] { "WhereLike", "WhereNotLike" };

            RegisterAction(Rule, context, accessedType, forbiddenMembers);
        }
    }
}
