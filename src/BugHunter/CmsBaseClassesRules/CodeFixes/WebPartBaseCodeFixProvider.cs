using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.CmsBaseClassesRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.CmsBaseClassesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WebPartBaseCodeFixProvider)), Shared]
    public class WebPartBaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(WebPartBaseAnalyzer.WEB_PART_DIAGNOSTIC_ID, WebPartBaseAnalyzer.UI_WEB_PART_DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private static readonly ClassAndItsNamespace[] UiWebPartBaseClasses =
{
            new ClassAndItsNamespace { ClassNamespace = "CMS.UIControls", ClassName = "CMSAbstractUIWebpart"},
        };

        private static readonly ClassAndItsNamespace[] WebPartBaseClasses =
        {
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractWebPart"},
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractEditableWebPart"},
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractLayoutWebPart"},
            new ClassAndItsNamespace { ClassNamespace = "CMS.PortalEngine.Web.UI", ClassName = "CMSAbstractWizardWebPart"},
            new ClassAndItsNamespace { ClassNamespace = "CMS.Ecommerce.Web.UI", ClassName = "CMSCheckoutWebPart"},
        };

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var baseTypeCodeFixHelper = new ClassDeclarationCodeFixHelper(context);

            var diagnostic = baseTypeCodeFixHelper.GetFirstDiagnostic(FixableDiagnosticIds.ToArray());
            var diagnosticId = diagnostic.Id;
            var classDeclaration = await baseTypeCodeFixHelper.GetDiagnosedClassDeclarationSyntax(diagnosticId);
            if (classDeclaration == null)
            {
                return;
            }

            var suggestions = diagnosticId == DiagnosticIds.UI_WEB_PART_BASE
                ? UiWebPartBaseClasses
                : WebPartBaseClasses;

            foreach (var classAndItsNamespace in suggestions)
            {
                var newClassDeclaration = classDeclaration.WithBaseClass(classAndItsNamespace.ClassName);
                context.RegisterCodeFix(
                   CodeAction.Create(
                       title: $"Inherit from {classAndItsNamespace.ClassName} instead",
                       createChangedDocument: c => baseTypeCodeFixHelper.ReplaceExpressionWith(classDeclaration, newClassDeclaration, classAndItsNamespace.ClassNamespace),
                       equivalenceKey: nameof(WebPartBaseCodeFixProvider) + classAndItsNamespace.ClassName),
                   diagnostic);
            }
        }
    }
}