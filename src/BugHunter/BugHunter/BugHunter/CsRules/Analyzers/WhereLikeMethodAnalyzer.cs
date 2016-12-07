using System.Collections.Immutable;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Location = Microsoft.CodeAnalysis.Location;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhereLikeMethodAnalyzer : BaseMemberInvocatoinAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.WHERE_LIKE_METHOD;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "WhereLike() or WhereNotLike() methods");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            var accessedType = typeof(CMS.DataEngine.WhereConditionBase<>);
            
            RegisterAction(Rule, context, accessedType, 
                nameof(CMS.DataEngine.WhereCondition.WhereLike), 
                nameof(CMS.DataEngine.WhereCondition.WhereNotLike));
        }

        protected override Location GetWarningLocation(InvocationExpressionSyntax invocationExpression)
        {
            return LocationHelper.GetLocationOfMethodInvocationOnly(invocationExpression);
        }
    }
}
