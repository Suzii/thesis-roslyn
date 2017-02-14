﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.SystemIoRules.Analyzers
{
    /// <summary>
    /// Searches for usages of <see cref="System.IO"/> and their access to anything other than <c>Exceptions</c> or <c>Stream</c>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SystemIOAnalyzer_V5_CompilationStartWithBagForDiagnosedNodes : DiagnosticAnalyzer
    {
        private static readonly string[] WhiteListedTypeNames =
        {
            "System.IO.IOException",
            "System.IO.Stream"
        };

        public const string DIAGNOSTIC_ID = DiagnosticIds.SYSTEM_IO;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_Title), SystemIoResources.ResourceManager, typeof(SystemIoResources)),
                messageFormat: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_MessageFormat), SystemIoResources.ResourceManager, typeof(SystemIoResources)),
                category: AnalyzerCategories.SystemIo,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(SystemIoResources.SystemIo_Description), SystemIoResources.ResourceManager, typeof(SystemIoResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly IDiagnosticFormatter DiagnosticFormatter = DiagnosticFormatterFactory.CreateDefaultFormatter();


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var whitelistedTypes = WhiteListedTypeNames
                    .Select(compilationStartAnalysisContext.Compilation.GetTypeByMetadataName)
                    .ToArray();

                var compilationaAnalyzer = new CompilationaAnalyzer(whitelistedTypes);

                compilationStartAnalysisContext.RegisterSyntaxNodeAction(nodeAnalyzsisContext => compilationaAnalyzer.Analyze(nodeAnalyzsisContext), SyntaxKind.IdentifierName);

                compilationStartAnalysisContext.RegisterCompilationEndAction(compilationEndContext => compilationaAnalyzer.Evaluate(compilationEndContext));
            });
        }

        class CompilationaAnalyzer
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
                if (!CheckPreConditions(context))
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

                if (_whitelistedTypes.Any(allowedType => symbol.ConstructedFrom.IsDerivedFromClassOrInterface(allowedType)))
                {
                    return;
                }

                _badNodes.Add(identifierNameSyntax);
            }

            private bool CheckPreConditions(SyntaxNodeAnalysisContext context)
            {
                // TODO check if file is generated
                return true;
            }

            public void Evaluate(CompilationAnalysisContext compilationEndContext)
            {
                foreach (var identifierNameSyntax in _badNodes)
                {
                    var diagnostic = CreateDiagnostic(Rule, identifierNameSyntax);
                    compilationEndContext.ReportDiagnostic(diagnostic);
                }
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
}
