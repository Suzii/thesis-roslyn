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
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and analyzing Symbol directly
    /// </summary>
    //[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class V11IdentifierNameEnhancedSyntaxAnalysisAndSymbolAnalysisWithCachedResults : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BHxV11";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DiagnosticId);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();
        private static readonly string[] WhiteListedIdentifierNames =
        {
            "System.IO.IOException",
            "System.IO.DirectoryNotFoundException",
            "System.IO.DriveNotFoundException",
            "System.IO.EndOfStreamException",
            "System.IO.FileLoadException",
            "System.IO.FileNotFoundException",
            "System.IO.PathTooLongException",
            "System.IO.PipeException",

            "System.IO.Stream",
            "Microsoft.JScript.COMCharStream",
            "System.Data.OracleClient.OracleBFile",
            "System.Data.OracleClient.OracleLob",
            "System.Data.SqlTypes.SqlFileStream",
            "System.IO.BufferedStream",
            "System.IO.Compression.DeflateStream",
            "System.IO.Compression.GZipStream",
            "System.IO.FileStream",
            "System.IO.MemoryStream",
            "System.IO.Pipes.PipeStream",
            "System.IO.UnmanagedMemoryStream",
            "System.Net.Security.AuthenticatedStream",
            "System.Net.Sockets.NetworkStream",
            "System.Printing.PrintQueueStream",
            "System.Security.Cryptography.CryptoStream",

            "System.IO.SeekOrigin"
        };

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

            if (IsWhitelisted(context, symbol))
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsWhitelisted(SyntaxNodeAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (WhiteListedIdentifierNames.Contains(symbol.ConstructedFrom.ToString()))
            {
                return true;
            }

            return symbol.ConstructedFrom.IsDerivedFrom("System.IO.IOException", context.Compilation) ||
                   symbol.ConstructedFrom.IsDerivedFrom("System.IO.SeekOrigin", context.Compilation) ||
                   symbol.ConstructedFrom.IsDerivedFrom("System.IO.Stream", context.Compilation);
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
