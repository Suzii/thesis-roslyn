using System.Collections.Immutable;
using BugHunter.Core.Constants;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.AbstractionOverImplementation.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LuceneSearchDocumentAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.LUCENE_SEARCH_DOCUMENT;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(AbstractionOverImplementationResources.LuceneSearchDocument_Title), AbstractionOverImplementationResources.ResourceManager, typeof(AbstractionOverImplementationResources)),
            messageFormat: new LocalizableResourceString(nameof(AbstractionOverImplementationResources.LuceneSearchDocument_MessageFormat), AbstractionOverImplementationResources.ResourceManager, typeof(AbstractionOverImplementationResources)),
            category: nameof(AnalyzerCategories.AbstractionOverImplementation),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(AbstractionOverImplementationResources.LuceneSearchDocument_Description), AbstractionOverImplementationResources.ResourceManager, typeof(AbstractionOverImplementationResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            const string forbiddenTypeFullyQualified = "CMS.Search.Lucene3.LuceneSearchDocument";
            const string forbiddenType = "LuceneSearchDocument";

            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
            {
                return;
            }

            var identifierName = identifierNameSyntax.Identifier.ToString();
            if (identifierName != forbiddenType)
            {
                return;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(identifierNameSyntax).Type as INamedTypeSymbol;
            if (actualTargetType == null || 
                !actualTargetType.IsDerivedFrom(forbiddenTypeFullyQualified, context.Compilation))
            {
                return;
            }
            
            // if direct parent is QualifiedName, surface diagnostic for whole QualifiedName
            var diagnosedNode = identifierNameSyntax.Parent.IsKind(SyntaxKind.QualifiedName)
                ? identifierNameSyntax.Parent
                : identifierNameSyntax;

            var warningLocation = diagnosedNode.GetLocation();
            var diagnostic = Diagnostic.Create(Rule, warningLocation, diagnosedNode);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
