using System.Collections.Immutable;
using BugHunter.Core.Constants;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.AbstractionOverImplementation.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LuceneSearchDocumentAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raised by <see cref="LuceneSearchDocumentAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.LuceneSearchDocument;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId,
            title: new LocalizableResourceString(nameof(AbstractionOverImplementationResources.LuceneSearchDocument_Title), AbstractionOverImplementationResources.ResourceManager, typeof(AbstractionOverImplementationResources)),
            messageFormat: new LocalizableResourceString(nameof(AbstractionOverImplementationResources.LuceneSearchDocument_MessageFormat), AbstractionOverImplementationResources.ResourceManager, typeof(AbstractionOverImplementationResources)),
            category: nameof(AnalyzerCategories.AbstractionOverImplementation),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(AbstractionOverImplementationResources.LuceneSearchDocument_Description), AbstractionOverImplementationResources.ResourceManager, typeof(AbstractionOverImplementationResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(DiagnosticId));

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
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
