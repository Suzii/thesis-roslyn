using System.Collections.Immutable;
using BugHunter.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BH1000MethodWhereLikeShouldNotBeUsed : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "BH1000";
        
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, 
            title: new LocalizableResourceString(nameof(CsResources.BH1000_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.BH1000_MessageFormat), CsResources.ResourceManager, typeof(CsResources)), 
            category: AnalyzerCategories.CsRules, 
            defaultSeverity: DiagnosticSeverity.Warning, 
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.BH1000_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;
            
            var memberName = memberAccessExpression.Name.ToString();
            if (memberName != "WhereLike" && memberName != "WhereNotLike")
            {
                return;
            }

            var searchedTargetType = TypesHelper.GetITypeSymbol(typeof(CMS.DataEngine.WhereConditionBase<>), context);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccessExpression) as INamedTypeSymbol;
            if (actualTargetType == null || !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType, true))
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation(), memberName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
