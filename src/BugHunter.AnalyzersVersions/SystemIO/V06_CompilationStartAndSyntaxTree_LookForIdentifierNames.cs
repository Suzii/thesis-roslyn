using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.SystemIO
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V06_CompilationStartAndSyntaxTree_LookForIdentifierNames : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V06";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var compilationAnalyzer = new CompilationaAnalyzer(compilationStartAnalysisContext.Compilation);

                compilationStartAnalysisContext.RegisterSyntaxTreeAction(systaxTreeContext => compilationAnalyzer.Analyze(systaxTreeContext));

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationAnalyzer.Evaluate(compilationEndContext));
            });
        }

        class CompilationaAnalyzer
        {
            private readonly Compilation _compilation;
            private readonly INamedTypeSymbol[] _whitelistedTypes;
            private readonly List<IdentifierNameSyntax> _badNodes;

            public CompilationaAnalyzer(Compilation compilation)
            {
                _compilation = compilation;

                _whitelistedTypes = AnalyzerHelper.WhiteListedTypeNames
                    .Select(compilation.GetTypeByMetadataName)
                    .ToArray();

                _badNodes = new List<IdentifierNameSyntax>();
            }

            public void Analyze(SyntaxTreeAnalysisContext context)
            {
                var syntaxTree = context.Tree;
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

                    if (_whitelistedTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFrom(allowedType)))
                    {
                        continue;
                    }

                    _badNodes.Add(identifierNameSyntax);
                }
            }

            public void Evaluate(CompilationAnalysisContext compilationEndContext)
            {
                Parallel.ForEach(_badNodes, (identifierNameSyntax) =>
                {
                    var diagnostic = AnalyzerHelper.CreateDiagnostic(Rule, identifierNameSyntax);
                    compilationEndContext.ReportDiagnostic(diagnostic);
                });
            }
        }
    }
}
