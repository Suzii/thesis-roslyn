using System;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.BaseClassesRules.Analyzers
{
    /// <summary>
    /// Checks if Web Part file inherits from right class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WebPartBaseAnalyzer : BaseClassDeclarationSyntaxAnalyzer
    {
        public const string WEB_PART_DIAGNOSTIC_ID = DiagnosticIds.WEB_PART_BASE;
        public const string UI_WEB_PART_DIAGNOSTIC_ID = DiagnosticIds.UI_WEB_PART_BASE;

        // TODO think of nicer messages
        private static readonly DiagnosticDescriptor WebPartRule = new DiagnosticDescriptor(WEB_PART_DIAGNOSTIC_ID,
                title: "Web Part must inherit the right class",
                messageFormat: "'{0}' should inherit from CMS<something>WebPart.",
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "Web Part must inherit the right class.");

        private static readonly DiagnosticDescriptor UiWebPartRule = new DiagnosticDescriptor(UI_WEB_PART_DIAGNOSTIC_ID,
        title: "UI Web Part must inherit the right class",
        messageFormat: "'{0}' should inherit from CMS<something>WebPart.",
        category: AnalyzerCategories.CS_RULES,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Web Part must inherit the right class.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(WebPartRule, UiWebPartRule);

        private static readonly string[] UiWebPartBases =
        {
            "CMS.UIControls.CMSAbstractUIWebpart"
        };

        private static readonly string[] WebPartBases =
        {
            "CMS.PortalEngine.Web.UI.CMSAbstractWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart",
            "CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart",
            "CMS.Ecommerce.Web.UI.CMSCheckoutWebPart"
        };


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                // leave here for performance optimization
                var uiWebPartBaseTypes = UiWebPartBases.Select(compilationContext.Compilation.GetTypeByMetadataName).ToArray();
                var webPartBaseTypes = WebPartBases.Select(compilationContext.Compilation.GetTypeByMetadataName).ToArray();

                if (uiWebPartBaseTypes.Union(webPartBaseTypes).Any(baseType => baseType == null))
                {
                    return;
                }

                compilationContext.RegisterSyntaxTreeAction(syntaxTreeAnalysisContext =>
                {
                    var filePath = syntaxTreeAnalysisContext.Tree.FilePath;
                    if (!FileIsInWebPartsFolder(filePath))
                    {
                        return;
                    }

                    var isUIWebPart = IsUIWebPart(filePath);
                    var enforcedWebPatBaseTypes = isUIWebPart ? uiWebPartBaseTypes : webPartBaseTypes;
                    var ruleToBeUsed = isUIWebPart ? UiWebPartRule : WebPartRule;

                    var publicInstantiableClassDeclarations = GetAllClassDeclarations(syntaxTreeAnalysisContext)
                        .Where(classDeclarationSyntax
                            => classDeclarationSyntax.IsPublic()
                            && !classDeclarationSyntax.IsAbstract());

                    var semanticModel = compilationContext.Compilation.GetSemanticModel(syntaxTreeAnalysisContext.Tree);

                    foreach (var classDeclaration in publicInstantiableClassDeclarations)
                    {
                        var baseTypeTypeSymbol = GetBaseTypeSymbol(classDeclaration, semanticModel);
                        if (baseTypeTypeSymbol == null)
                        {
                            continue;
                        }

                        // if class inherits from one of enforced base types, skip
                        if (enforcedWebPatBaseTypes.Any(enforcedWebPartBase => baseTypeTypeSymbol.Equals(enforcedWebPartBase)))
                        {
                            continue;
                        }

                        var diagnostic = CreateDiagnostic(syntaxTreeAnalysisContext, classDeclaration, ruleToBeUsed);
                        syntaxTreeAnalysisContext.ReportDiagnostic(diagnostic);
                    }
                });
            });
        }
        
        private static bool FileIsInWebPartsFolder(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) &&
                   !filePath.Contains("_files\\") &&
                   (filePath.Contains(FilePaths.Folders.UI_WEB_PARTS) || filePath.Contains(FilePaths.Folders.WEB_PARTS));
        }
        
        /// <summary>
        /// Checks if the given file is UI web part.
        /// </summary>
        /// <param name="path">Path to web part file.</param>
        private static bool IsUIWebPart(string path)
        {
            return !string.IsNullOrEmpty(path) && path.IndexOf(FilePaths.Folders.UI_WEB_PARTS, StringComparison.OrdinalIgnoreCase) > -1;
        }
    }
}
