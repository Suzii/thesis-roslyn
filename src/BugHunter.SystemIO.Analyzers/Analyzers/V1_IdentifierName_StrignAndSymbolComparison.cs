using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.SystemIO.Analyzers.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and using SemanticModelBrowser
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class V1_IdentifierName_StrignAndSymbolComparison : DiagnosticAnalyzer
    {
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
        };

        public const string DIAGNOSTIC_ID = "V0";

        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DIAGNOSTIC_ID);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterSyntaxNodeAction(c => Analyze(Rule, c), SyntaxKind.IdentifierName);
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null || identifierNameSyntax.IsVar)
            {
                return;
            }

            var identifierNameTypeSymbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as INamedTypeSymbol;
            if (identifierNameTypeSymbol == null)
            {
                return;
            }

            var symbolContainingNamespace = identifierNameTypeSymbol.ContainingNamespace;
            if (!symbolContainingNamespace.ToString().Equals("System.IO"))
            {
                return;
            }

            if (IsWhiteListed(context, identifierNameTypeSymbol))
            {
                return;
            }

            var diagnostic = AnalyzerHelper.CreateDiagnostic(rule, identifierNameSyntax);

            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsWhiteListed(SyntaxNodeAnalysisContext context, INamedTypeSymbol identifierNameTypeSymbol)
        {
            if (identifierNameTypeSymbol != null && WhiteListedIdentifierNames.Contains(identifierNameTypeSymbol.ToString()))
            {
                return true;
            }

            return
                AnalyzerHelper.WhiteListedTypeNames.Any(
                    whiteListedType =>
                        identifierNameTypeSymbol.IsDerivedFromClassOrInterface(
                            context.SemanticModel.Compilation.GetTypeByMetadataName(whiteListedType)));
        }
    }
}
