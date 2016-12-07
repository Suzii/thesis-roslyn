using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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
            var accessedType = typeof(CMS.DataEngine.WhereConditionBase<>);
            
            RegisterAction(Rule, context, accessedType, nameof(CMS.DataEngine.WhereCondition.WhereLike), nameof(CMS.DataEngine.WhereCondition.WhereNotLike));
        }
    }
}
