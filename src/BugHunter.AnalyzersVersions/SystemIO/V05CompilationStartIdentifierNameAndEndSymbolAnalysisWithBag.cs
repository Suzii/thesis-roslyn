using System.Collections.Generic;
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
    public class V05CompilationStartIdentifierNameAndEndSymbolAnalysisWithBag : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BHxV05";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DiagnosticId);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var whitelistedTypes = AnalyzerHelper.WhiteListedTypeNames
                    .Select(compilationStartAnalysisContext.Compilation.GetTypeByMetadataName)
                    .ToArray();

                var compilationAnalyzer = new CompilationAnalyzer(whitelistedTypes);

                compilationStartAnalysisContext.RegisterSyntaxNodeAction(nodeAnalysisContext => compilationAnalyzer.Analyze(nodeAnalysisContext), SyntaxKind.IdentifierName);

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationAnalyzer.Evaluate(compilationEndContext));
            });
        }

        private class CompilationAnalyzer
        {
            private readonly INamedTypeSymbol[] _whitelistedTypes;
            private readonly List<IdentifierNameSyntax> _badNodes;

            public CompilationAnalyzer(INamedTypeSymbol[] whitelistedTypes)
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
                    var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
                    compilationEndContext.ReportDiagnostic(diagnostic);
                });
            }
        }
    }
}
