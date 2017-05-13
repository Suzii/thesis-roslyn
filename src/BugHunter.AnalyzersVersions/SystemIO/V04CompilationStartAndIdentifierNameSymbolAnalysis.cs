using System.Collections.Immutable;
using System.Linq;
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
    public class V04CompilationStartAndIdentifierNameSymbolAnalysis : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BHxV04";
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

                compilationStartAnalysisContext.RegisterSyntaxNodeAction(nodeAnalysisContext => Analyze(nodeAnalysisContext, whitelistedTypes), SyntaxKind.IdentifierName);
            });
        }

        private static void Analyze(SyntaxNodeAnalysisContext context, INamedTypeSymbol[] allowedSystemIoTypes)
        {
            var syntaxNode = context.SemanticModel.SyntaxTree.GetRoot();
            if (!syntaxNode.ToString().Contains(".IO"))
            {
                return;
            }

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

            if (allowedSystemIoTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFrom(allowedType)))
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
