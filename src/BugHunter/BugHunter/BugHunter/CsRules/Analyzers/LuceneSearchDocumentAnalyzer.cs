using System.Collections.Immutable;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LuceneSearchDocumentAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.LUCERNE_SEARCH_DOCUMENT;

        private static readonly DiagnosticDescriptor Rule = ApiReplacementRuleBuilder.GetRule(DIAGNOSTIC_ID, "LucerneSearchDocument", "ISearchDocument");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var forbiddenTypeFullyQualified = "CMS.Search.Lucene3.LuceneSearchDocument";
            var forbiddenType = "LuceneSearchDocument";
            var identifierNameNode = (IdentifierNameSyntax)context.Node;
            var identifierName = identifierNameNode.Identifier.ToString();
            if (identifierName != forbiddenType)
            {
                return;
            }

            var searchedTargetType = TypeExtensions.GetITypeSymbol(forbiddenTypeFullyQualified, context);
            var actualTargetTypeInfo = context.SemanticModel.GetTypeInfo(identifierNameNode);
            var actualTargetType = actualTargetTypeInfo.Type;
            if (searchedTargetType == null || actualTargetType == null || !(actualTargetType as INamedTypeSymbol).IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return;
            }

            var warningLocation = identifierNameNode.GetLocation();
            var diagnostic = Diagnostic.Create(Rule, warningLocation, identifierName);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
