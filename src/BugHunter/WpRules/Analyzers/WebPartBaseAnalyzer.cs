using System;
using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.WpRules.Analyzers
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
            context.RegisterSyntaxTreeAction(Analyze);
        }

        private void Analyze(SyntaxTreeAnalysisContext context)
        {
            var filePath = context.Tree.FilePath;
            if (string.IsNullOrEmpty(filePath) || filePath.Contains("_files\\") || !(filePath.Contains(ProjectPaths.UI_WEB_PARTS) || filePath.Contains(ProjectPaths.WEB_PARTS)))
            {
                return;
            }

            string[] webPartBases;
            if (IsUIWebPart(filePath))
            {
                // UI web part can be inherited form CMSAbstractUIWebpart only
                webPartBases = new[] { "CMSAbstractUIWebpart" };
            }
            else
            {
                webPartBases = new[] {
                "CMSAbstractWebPart",
                "CMSAbstractEditableWebPart",
                "CMSAbstractLayoutWebPart",
                "CMSAbstractLanguageWebPart",
                "CMSCheckoutWebPart",
                "CMSAbstractWireframeWebPart",
                "SocialMediaAbstractWebPart",
                "CMSAbstractWizardWebPart",
                //"CMS.PortalEngine.Web.UI.CMSAbstractWebPart",
                //"CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart",
                //"CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart",
                //"CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart",
                //"CMS.Ecommerce.Web.UI.CMSCheckoutWebPart"
                };
            }

            var root = context.Tree.GetRoot();
            var publicInstantiableClassDeclarations = root
                .DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .Where(classDeclarationSyntax
                    => IsPublicClass(classDeclarationSyntax)
                    && !IsAbstractCLass(classDeclarationSyntax));

            var classDeclarationsNotExtendingCMSWebPart = publicInstantiableClassDeclarations
                .Where(classDeclaration => !ExtendsRequiredCMSWebPart(classDeclaration, webPartBases))
                .ToList();

            if (classDeclarationsNotExtendingCMSWebPart.Count == 0)
            {
                return;
            }

            foreach (var badWebPart in classDeclarationsNotExtendingCMSWebPart)
            {
                var location = context.Tree.GetLocation(badWebPart.Identifier.FullSpan);
                var diagnostic = Diagnostic.Create(Rule, location, badWebPart.Identifier.ToString());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool ExtendsRequiredCMSWebPart(ClassDeclarationSyntax classDeclaration, string[] webPartBases)
        {
            var baseList = classDeclaration.BaseList;
            return baseList != null && baseList.Types.Any(baseType => IsOneOfRequiredCMSWebParts(baseType, webPartBases));
        }

        private static bool IsOneOfRequiredCMSWebParts(BaseTypeSyntax baseType, string[] cmsWebPartBases)
        {
            var baseTypeName = baseType.ToString();
            var baseTypeClassName = baseTypeName.Substring(1 + baseTypeName.LastIndexOf(".", StringComparison.Ordinal));
            // TODO
            return cmsWebPartBases.Any(webPartBase => webPartBase == baseTypeClassName);
        }

        private static bool IsAbstractCLass(ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.AbstractKeyword));
        }

        private static bool IsPublicClass(ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));
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
