using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.SystemIoRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemIOAnalyzer_V7_CompilationStartAndSyntaxTreeAndFulltextSearch : DiagnosticAnalyzer
    {
        private static readonly string[] WhiteListedTypeNames =
        {
            "System.IO.IOException",
            "System.IO.Stream"
        };

        public const string DIAGNOSTIC_ID = DiagnosticIds.SYSTEM_IO;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_Title), SystemIoResources.ResourceManager, typeof(SystemIoResources)),
                messageFormat: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_MessageFormat), SystemIoResources.ResourceManager, typeof(SystemIoResources)),
                category: AnalyzerCategories.SystemIo,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_Description), SystemIoResources.ResourceManager, typeof(SystemIoResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var compilationaAnalyzer = new CompilationAnalyzer(compilationStartAnalysisContext.Compilation);

                compilationStartAnalysisContext.RegisterSyntaxTreeAction(systaxTreeContext => compilationaAnalyzer.Analyze(systaxTreeContext));

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationaAnalyzer.Evaluate(compilationEndContext));
            });
        }

        class CompilationAnalyzer
        {
            private readonly Compilation _compilation;
            private readonly INamedTypeSymbol[] _whitelistedTypes;
            private readonly List<IdentifierNameSyntax> _badNodes;

            public CompilationAnalyzer(Compilation compilation)
            {
                _compilation = compilation;

                _whitelistedTypes = WhiteListedTypeNames
                    .Select(compilation.GetTypeByMetadataName)
                    .ToArray();

                _badNodes = new List<IdentifierNameSyntax>();
            }

            public void Analyze(SyntaxTreeAnalysisContext context)
            {
                if (!CheckPreConditions(context))
                {
                    return;
                }

                var syntaxTree = context.Tree;

                if (!syntaxTree.ToString().Contains(".IO"))
                {
                    return;
                }

                var identifierNameSyntaxs = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<IdentifierNameSyntax>();
                var semanticModel = _compilation.GetSemanticModel(syntaxTree);

                foreach (var identifierNameSyntax in identifierNameSyntaxs)
                {
                    if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
                    {
                        continue;
                    }

                    var symbol = semanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
                    if (symbol == null)
                    {
                        continue;
                    }

                    var symbolContainingNamespace = symbol.ContainingNamespace;
                    if (!symbolContainingNamespace.ToString().Equals("System.IO"))
                    {
                        continue;
                    }

                    if (_whitelistedTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFromClassOrInterface(allowedType)))
                    {
                        continue;
                    }

                    _badNodes.Add(identifierNameSyntax);
                }
            }

            private bool CheckPreConditions(SyntaxTreeAnalysisContext context)
            {
                // TODO check if file is generated
                return true;
            }

            public void Evaluate(CompilationAnalysisContext compilationEndContext)
            {
                foreach (var identifierNameSyntax in _badNodes)
                {
                    var diagnostic = CreateDiagnostic(Rule, identifierNameSyntax);
                    compilationEndContext.ReportDiagnostic(diagnostic);
                }
            }

            private Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, IdentifierNameSyntax identifierName)
            {
                var rootIdentifierName = identifierName.AncestorsAndSelf().Last(n => n.IsKind(SyntaxKind.QualifiedName) || n.IsKind(SyntaxKind.IdentifierName));
                var diagnosedNode = rootIdentifierName;
                while (diagnosedNode?.Parent != null && (diagnosedNode.Parent.IsKind(SyntaxKind.ObjectCreationExpression) ||
                                                         diagnosedNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                         diagnosedNode.Parent.IsKind(SyntaxKind.InvocationExpression)))
                {
                    diagnosedNode = diagnosedNode.Parent as ExpressionSyntax;
                }

                var usedAs = DiagnosticFormatter.GetDiagnosedUsage(diagnosedNode);
                var location = DiagnosticFormatter.GetLocation(diagnosedNode);

                return Diagnostic.Create(rule, location, usedAs);
            }
        }
    }
}
