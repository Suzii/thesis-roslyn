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
    /// 
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and using SemanticModelBrowser
    /// </summary>
    //[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class V01IdentifierNameStringAndSymbolComparison : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
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

            "System.IO.SeekOrigin"
        };

        public const string DiagnosticId = "BHxV01";
        private static readonly DiagnosticDescriptor Rule = AnalyzerHelper.GetRule(DiagnosticId);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        private static readonly ISyntaxNodeDiagnosticFormatter<IdentifierNameSyntax> DiagnosticFormatter = new SystemIoDiagnosticFormatter();

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

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, identifierNameSyntax);

            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsWhiteListed(SyntaxNodeAnalysisContext context, INamedTypeSymbol symbol)
        {
            if (WhiteListedIdentifierNames.Contains(symbol.ConstructedFrom.ToString()))
            {
                return true;
            }

            return symbol.ConstructedFrom.IsDerivedFrom("System.IO.IOException", context.Compilation) ||
                   symbol.ConstructedFrom.IsDerivedFrom("System.IO.SeekOrigin", context.Compilation) ||
                   symbol.ConstructedFrom.IsDerivedFrom("System.IO.Stream", context.Compilation);
        }
    }
}
