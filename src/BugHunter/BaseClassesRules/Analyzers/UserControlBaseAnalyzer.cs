﻿using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.BaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if User Control file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UserControlBaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.USER_CONTROL_BASE;

        // TODO think of nicer messages
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "User control must inherit the right class",
                messageFormat: "'{0}' should inherit from some abstract CMSControl.",
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "User control must inherit the right class.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var systemWebUiControlType = compilationContext.Compilation.GetTypeByMetadataName("System.Web.UI.Control");
                if (systemWebUiControlType == null)
                {
                    return;
                }

                compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
                {

                    var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
                    if (string.IsNullOrEmpty(filePath) || !filePath.Contains(ProjectPaths.USER_CONTROLS))
                    {
                        return;
                    }

                    var publicPartialInstantiableClassDeclarations = syntaxTreeAnalysisContext
                        .Tree
                        .GetRoot()
                        .DescendantNodesAndSelf()
                        .OfType<ClassDeclarationSyntax>()
                        .Where(classDeclarationSyntax
                            => classDeclarationSyntax.IsPublic()
                            && !classDeclarationSyntax.IsAbstract()
                            && classDeclarationSyntax.IsPartial());

                    foreach (var classDeclaration in publicPartialInstantiableClassDeclarations)
                    {
                        if (classDeclaration.BaseList == null || classDeclaration.BaseList.Types.IsNullOrEmpty())
                        {
                            continue;
                        }

                        var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);
                        var baseTypeTypeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration).BaseType;
                        if (baseTypeTypeSymbol == null || !baseTypeTypeSymbol.Equals(systemWebUiControlType))
                        {
                            continue;
                        }

                        var diagnostic = GetDiagnostic(syntaxTreeAnalysisContext, classDeclaration);
                        syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
                    }
                });
            });
        }

        private static Diagnostic GetDiagnostic(SyntaxTreeAnalysisContext syntaxTreeAnalysisContext, ClassDeclarationSyntax classDeclaration)
        {
            var location = syntaxTreeAnalysisContext.Tree.GetLocation(classDeclaration.Identifier.FullSpan);
            var diagnostic = Diagnostic.Create(Rule, location, classDeclaration.Identifier.ToString());
            return diagnostic;
        }
    }
}
