using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.SystemIO.Analyzers.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V04_CompilationStartAndIdentifierName_SymbolAnalysis : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V04";
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

            if (allowedSystemIoTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFromClassOrInterface(allowedType)))
            {
                return;
            }

            var diagnostic = AnalyzerHelper.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
