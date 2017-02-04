using System;
using System.Collections.Immutable;
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
    /// Checks if Web Part file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebPartBaseAnalyzer : DiagnosticAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.WEB_PART_BASE;

        // TODO think of nicer messages
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
                title: "Web Part must inherit the right class",
                messageFormat: "'{0}' should inherit from CMS<something>WebPart.",
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "Web Part must inherit the right class.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                // UI web part can be inherited form CMSAbstractUIWebpart only
                var uiWebPartBases = new[]
                    {
                        "CMS.UIControls.CMSAbstractUIWebpart"
                    }
                    .Select(compilationContext.Compilation.GetTypeByMetadataName);

                var webPartBases = new[]
                    {
                        //"CMSAbstractWebPart",
                        //"CMSAbstractEditableWebPart",
                        //"CMSAbstractLayoutWebPart",
                        //"CMSAbstractLanguageWebPart",
                        //"CMSCheckoutWebPart",
                        //"CMSAbstractWireframeWebPart",
                        //"SocialMediaAbstractWebPart",
                        //"CMSAbstractWizardWebPart",
                        "CMS.PortalEngine.Web.UI.CMSAbstractWebPart",
                        "CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart",
                        "CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart",
                        "CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart",
                        "CMS.Ecommerce.Web.UI.CMSCheckoutWebPart"
                    }
                    .Select(compilationContext.Compilation.GetTypeByMetadataName);

                if (uiWebPartBases.Union(webPartBases).Any(baseType => baseType == null))
                {
                    return;
                }

                compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
                {
                    var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
                    if (string.IsNullOrEmpty(filePath) ||
                        filePath.Contains("_files\\") ||
                        !(filePath.Contains(ProjectPaths.UI_WEB_PARTS) || filePath.Contains(ProjectPaths.WEB_PARTS)))
                    {
                        return;
                    }

                    var publicInstantiableClassDeclarations = syntaxTreeAnalysisContext
                        .Tree
                        .GetRoot()
                        .DescendantNodesAndSelf()
                        .OfType<ClassDeclarationSyntax>()
                        .Where(classDeclarationSyntax
                            => classDeclarationSyntax.IsPublic()
                            && !classDeclarationSyntax.IsAbstract());

                    foreach (var classDeclaration in publicInstantiableClassDeclarations)
                    {
                        if (classDeclaration.BaseList != null && !classDeclaration.BaseList.Types.IsNullOrEmpty())
                        {
                            var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);
                            var baseTypeTypeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration).BaseType;

                            if (baseTypeTypeSymbol != null)
                            {
                                if (IsUIWebPart(filePath))
                                {
                                    if (uiWebPartBases.Any(baseTypeTypeSymbol.Equals))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (webPartBases.Any(baseTypeTypeSymbol.Equals))
                                    {
                                        continue;
                                    }
                                }
                            }
                        }

                        var diagnostic = CreateDIagnostic(syntaxTreeAnalysisContext, classDeclaration);
                        syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
                    }
                });
            });
        }

        private static Diagnostic CreateDIagnostic(SyntaxTreeAnalysisContext syntaxTreeAnalysisContext, ClassDeclarationSyntax classDeclaration)
        {
            var location = syntaxTreeAnalysisContext.Tree.GetLocation(classDeclaration.Identifier.FullSpan);
            var diagnostic = Diagnostic.Create(Rule, location, classDeclaration.Identifier.ToString());
            return diagnostic;
        }

        /// <summary>
        /// Checks if the given file is UI web part.
        /// </summary>
        /// <param name="path">Path to web part file.</param>
        private static bool IsUIWebPart(string path)
        {
            return (!string.IsNullOrEmpty(path) && path.IndexOf(ProjectPaths.UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) > -1);
        }
    }
}
