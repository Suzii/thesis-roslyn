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
    public class V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V05";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var whitelistedTypes = AnalyzerHelper.WhiteListedTypeNames
                    .Select(compilationStartAnalysisContext.Compilation.GetTypeByMetadataName)
                    .ToArray();

                var compilationaAnalyzer = new CompilationaAnalyzer(whitelistedTypes);

                compilationStartAnalysisContext.RegisterSyntaxNodeAction(nodeAnalyzsisContext => compilationaAnalyzer.Analyze(nodeAnalyzsisContext), SyntaxKind.IdentifierName);

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationaAnalyzer.Evaluate(compilationEndContext));
            });
        }

        private class CompilationaAnalyzer
        {
            private readonly INamedTypeSymbol[] _whitelistedTypes;
            private readonly List<IdentifierNameSyntax> _badNodes;


            public CompilationaAnalyzer(INamedTypeSymbol[] whitelistedTypes)
            {
                _whitelistedTypes = whitelistedTypes;
                _badNodes = new List<IdentifierNameSyntax>();
            }

            public void Analyze(SyntaxNodeAnalysisContext context)
            {
                var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
                if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
                {
                    return;
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

                if (_whitelistedTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFrom(allowedType)))
                {
                    return;
                }

                _badNodes.Add(identifierNameSyntax);
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
