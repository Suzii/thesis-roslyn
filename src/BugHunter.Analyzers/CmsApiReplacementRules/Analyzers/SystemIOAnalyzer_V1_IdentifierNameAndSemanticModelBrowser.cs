using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// 
    /// Version with callback on IdentifierName and using SemanticModelBrowser
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemIOAnalyzer_V1_IdentifierNameAndSemanticModelBrowser : DiagnosticAnalyzer
    {
        private static readonly string[] WhiteListedTypes =
        {
            "System.IO.IOException",
            "System.IO.Stream"
        };

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

        public const string DIAGNOSTIC_ID = DiagnosticIds.SYSTEM_IO;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_Title), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
                messageFormat: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_MessageFormat), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(CmsApiReplacementsResources.SystemIo_Description), CmsApiReplacementsResources.ResourceManager, typeof(CmsApiReplacementsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(c => Analyze(Rule, c), SyntaxKind.IdentifierName);
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context)
        {
            var identifierNameSyntax = (IdentifierNameSyntax)context.Node;
            if (identifierNameSyntax == null)
            {
                return;
            }

            if (identifierNameSyntax.IsVar || !(context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol is ITypeSymbol))
            {
                return;
            }


            var semanticModelBrowser = new SemanticModelBrowser(context);
            var identifierNameTypeSymbol = semanticModelBrowser.GetNamedTypeSymbol(identifierNameSyntax);
            if (identifierNameTypeSymbol == null)
            {
                return;
            }

            if (!CheckPreConditions(context) ||
                !IsInSystemIONamespace(context, identifierNameTypeSymbol) ||
                IsWhiteListed(context, identifierNameTypeSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(rule, identifierNameSyntax);

            context.ReportDiagnostic(diagnostic);
        }

        private bool IsInSystemIONamespace(SyntaxNodeAnalysisContext context, INamedTypeSymbol identifierNameTypeSymbol)
        {
            return identifierNameTypeSymbol.ContainingNamespace.ToString().Equals("System.IO");
        }

        private bool IsWhiteListed(SyntaxNodeAnalysisContext context, INamedTypeSymbol identifierNameTypeSymbol)
        {
            if (identifierNameTypeSymbol != null && WhiteListedIdentifierNames.Contains(identifierNameTypeSymbol.ToString()))
            {
                return true;
            }

            return
                WhiteListedTypes.Any(
                    whiteListedType =>
                        identifierNameTypeSymbol.IsDerivedFromClassOrInterface(
                            context.SemanticModel.Compilation.GetTypeByMetadataName(whiteListedType)));
        }

        private bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            // TODO check if file is generated
            return true;
        }

        private Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, IdentifierNameSyntax identifierName)
        {
            var rootIdentifierName = identifierName.AncestorsAndSelf().Last(n => n.IsKind(SyntaxKind.QualifiedName) || n.IsKind(SyntaxKind.IdentifierName));
            var diagnosedNode = rootIdentifierName;
            while (diagnosedNode?.Parent != null && (diagnosedNode.Parent.IsKind(SyntaxKind.ObjectCreationExpression) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                     diagnosedNode.Parent.IsKind(SyntaxKind.InvocationExpression)))
            {
                diagnosedNode = diagnosedNode.Parent as ExpressionSyntax;
            }

            var usedAs = DiagnosticFormatter.GetDiagnosedUsage(diagnosedNode);
            var location = DiagnosticFormatter.GetLocation(diagnosedNode);

            return Diagnostic.Create(rule, location, usedAs);
        }
    }
}
