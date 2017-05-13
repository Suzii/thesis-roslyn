using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.AnalyzersVersions.SystemIO.Helpers;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.SystemIO
{
    /// <summary>
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// </summary>
    // [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class V08CompilationStartSyntaxTreeAndEndFulltextSearchAndSymbolParallelAnallysisWithBag : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BHxV08";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DiagnosticId);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();

        public override void Initialize(AnalysisContext context)
        {
            // context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var compilationAnalyzer = new CompilationAnalyzer(compilationStartAnalysisContext.Compilation);

                compilationStartAnalysisContext.RegisterSyntaxTreeAction(systaxTreeContext => compilationAnalyzer.Analyze(systaxTreeContext));

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationAnalyzer.Evaluate(compilationEndContext));
            });
        }

        private class CompilationAnalyzer
        {
            private readonly Compilation _compilation;
            private readonly INamedTypeSymbol[] _whitelistedTypes;
            private readonly ConcurrentBag<IdentifierNameSyntax> _badNodes;

            public CompilationAnalyzer(Compilation compilation)
            {
                _compilation = compilation;

                _whitelistedTypes = AnalyzerHelper.WhiteListedTypeNames
                    .Select(compilation.GetTypeByMetadataName)
                    .ToArray();

                _badNodes = new ConcurrentBag<IdentifierNameSyntax>();
            }

            public void Analyze(SyntaxTreeAnalysisContext context)
            {
                var syntaxTree = context.Tree;

                if (!syntaxTree.ToString().Contains(".IO"))
                {
                    return;
                }

                var identifierNameSyntaxes = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<IdentifierNameSyntax>();
                var semanticModel = _compilation.GetSemanticModel(syntaxTree);

                Parallel.ForEach(identifierNameSyntaxes, (identifierNameSyntax) =>
                {
                    if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
                    {
                        return;
                    }

                    var symbol = semanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
                    if (symbol == null)
                    {
                        return;
                    }

                    var symbolContainingNamespace = symbol.ContainingNamespace;
                    if (!symbolContainingNamespace.ToString().Equals("System.IO"))
                    {
                        return;
                    }

                    if (_whitelistedTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFrom(allowedType)))
                    {
                        return;
                    }

                    _badNodes.Add(identifierNameSyntax);
                });
            }

            public void Evaluate(CompilationAnalysisContext compilationEndContext)
            {
                Parallel.ForEach(_badNodes, (identifierNameSyntax) =>
                {
                    var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
                    compilationEndContext.ReportDiagnostic(diagnostic);
                });
            }
        }
    }
}
