using System.Collections.Immutable;
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
        private const string CATEGORY = AnalyzerCategories.CsRules;

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(CsResources.BH1000_Title), CsResources.ResourceManager, typeof(CsResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(CsResources.BH1000_MessageFormat), CsResources.ResourceManager, typeof(CsResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(CsResources.BH1000_Description), CsResources.ResourceManager, typeof(CsResources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID, Title, MessageFormat, CATEGORY, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleMemberAccessExpression);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)context.Node;
            
            // First, filter on syntax only
            var memberName = memberAccessExpression.Name.ToString();
            if (memberName != "WhereLike" && memberName != "WhereNotLike")
            {
                return;
            }

            // Then look at semantics
            var memberSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpression).Symbol as IMethodSymbol;
            if (memberSymbol == null)
            {
                return;
            }

            var containingNamespace = memberSymbol.ContainingNamespace;
            if (containingNamespace.ToDisplayString() != "CMS.DataEngine")
            {
                return;
            }

            // TODO: check if accessed type can be derived from typeof(WhereConditionBase<>)??
            
            var diagnostic = Diagnostic.Create(Rule, memberAccessExpression.GetLocation(), memberName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
