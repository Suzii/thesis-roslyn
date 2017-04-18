using System.Collections.Concurrent;
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
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and analyzing Symbol directly
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V11_IdentifierName_EnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = "V11";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();

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
                var rootIdentifierName = identifierNameSyntax.GetOuterMostParentOfDottedExpression();

                // if not System.IO or if exception or string are used, return
                var rootIdentifierNameText = rootIdentifierName.ToString();
                if (!rootIdentifierNameText.Contains("System.IO")
                    || rootIdentifierNameText.Contains("IOException")
                    || rootIdentifierNameText.Contains("Stream")
                    || rootIdentifierNameText.Contains("SeekOrigin"))
                {
                    return;
                }
            }

            var symbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (symbol == null)
            {
                return;
            }

            // leave like this, not StartsWith() - e.g. System.IO.Compression is okay
            var symbolContainingNamespace = symbol.ContainingNamespace;
            if (!symbolContainingNamespace.ToString().Equals("System.IO"))
            {
                return;
            }

            if (symbol.ConstructedFrom.IsDerivedFrom("System.IO.IOException", context.Compilation) ||
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.Stream", context.Compilation) ||
                symbol.ConstructedFrom.IsDerivedFrom("System.IO.SeekOrigin", context.Compilation))
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
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
