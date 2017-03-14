using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and using SemanticModelBrowser
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemIOAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.SYSTEM_IO;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_Title), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_MessageFormat), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_Description), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationStartContext =>
            {
                var filesWithSystemIoUsing = new ConcurrentDictionary<string, bool?>();

                compilationStartContext.RegisterSyntaxNodeAction(c => Analyze(c, filesWithSystemIoUsing), SyntaxKind.IdentifierName);
            });
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, ConcurrentDictionary<string, bool?> filesWithSystemIoUsing)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
            {
                return;
            }

            // quick only syntax based analysis
            var fileContainsSystemIoUsing = FileContainsSystemIoUsing(identifierNameSyntax, filesWithSystemIoUsing);
            if (fileContainsSystemIoUsing.HasValue && !fileContainsSystemIoUsing.Value)
            {
                // identifier name must be fully qualified - look for System.IO there
                var rootIdentifierName = GetOuterMostParentOfDottedExpression(identifierNameSyntax);

                // if not System.IO or if exception or string are used, return
                var rootIdentifierNameText = rootIdentifierName.ToString();
                if (!rootIdentifierNameText.Contains("System.IO")
                    || rootIdentifierNameText.Contains("IOException")
                    || rootIdentifierNameText.Contains("Stream"))
                {
                    return;
                }
            }

            var symbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (symbol == null)
            {
                return;
            }

            var symbolContainingNamespace = symbol.ContainingNamespace;
            if (!symbolContainingNamespace.ToString().Equals("System.IO"))
            {
                return;
            }
            
            if (symbol.ConstructedFrom.IsDerivedFrom("System.IO.IOException", context.Compilation) || 
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.Stream", context.Compilation))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }

        private static Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, IdentifierNameSyntax identifierName)
        {
            var rootOfDottedExpression = GetOuterMostParentOfDottedExpression(identifierName);
            var diagnosedNode = rootOfDottedExpression.Parent.IsKind(SyntaxKind.ObjectCreationExpression)
                ? rootOfDottedExpression.Parent
                : rootOfDottedExpression;

            var usedAs = DiagnosticFormatter.GetDiagnosedUsage(diagnosedNode);
            var location = DiagnosticFormatter.GetLocation(diagnosedNode);

            return Diagnostic.Create(rule, location, usedAs);
        }

        // TODO add unit tests
        private static SyntaxNode GetOuterMostParentOfDottedExpression(IdentifierNameSyntax identifierNameSyntax)
        {
            SyntaxNode diagnosedNode = identifierNameSyntax;
            while (diagnosedNode?.Parent != null && (diagnosedNode.Parent.IsKind(SyntaxKind.QualifiedName) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.InvocationExpression)))
            {
                diagnosedNode = diagnosedNode.Parent;
            }

            return diagnosedNode;
        }

        private static bool? FileContainsSystemIoUsing(SyntaxNode identifierNameSyntax,
            ConcurrentDictionary<string, bool?> filesWithSystemIoUsing)
        {
            var syntaxTree = identifierNameSyntax.SyntaxTree;

            bool? result;
            var contains = filesWithSystemIoUsing.TryGetValue(syntaxTree.FilePath, out result);

            if (contains)
            {
                return result;
            }

            var root = syntaxTree.HasCompilationUnitRoot ? syntaxTree.GetCompilationUnitRoot() : null;

            if (root == null)
            {
                return filesWithSystemIoUsing
                    .AddOrUpdate(syntaxTree.FilePath, key => null, (key, originalVal) => null);
            }

            var containsSystemIoUsing = root
                .Usings
                .Any(u => u.ToString().Contains("System.IO"));

            return filesWithSystemIoUsing
                .AddOrUpdate(syntaxTree.FilePath, key => containsSystemIoUsing, (key, originalVal) => containsSystemIoUsing);
        }
    }
}
