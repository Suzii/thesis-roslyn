using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.SystemIO
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and analyzing Symbol directly
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V10_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysis : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V10";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly ISyntaxNodeDiagnosticFormatter<SyntaxNode> DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName);
        }

        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
            {
                return;
            }

            // quick only syntax based analysis
            if (!FileContainsSystemIoUsing(identifierNameSyntax))
            {
                // identifier name must be fully qualified - look for System.IO there
                var rootIdentifierName = GetOuterMostParentOfDottedExpression(identifierNameSyntax);

                // if not System.IO or if exception or string are used, return
                if (!rootIdentifierName.ToString().Contains("System.IO")
                    || rootIdentifierName.ToString().Contains("IOException")
                    || rootIdentifierName.ToString().Contains("Stream"))
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

            return DiagnosticFormatter.CreateDiagnostic(rule, diagnosedNode);
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

        private static bool FileContainsSystemIoUsing(SyntaxNode identifierNameSyntax)
        {
            var allUsings = identifierNameSyntax
                .AncestorsAndSelf()
                .OfType<CompilationUnitSyntax>()
                .SingleOrDefault()
                .Usings;

            var systemIoUsings = allUsings
                .Where(u => u.ToString().Contains("System.IO"));

            return systemIoUsings.Any();
        }
    }
}
