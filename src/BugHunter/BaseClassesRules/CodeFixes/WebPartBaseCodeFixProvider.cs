using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.BaseClassesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WebPartBaseCodeFixProvider)), Shared]
    public class WebPartBaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(WebPartBaseAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

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

            var diagnostic = baseTypeCodeFixHelper.GetFirstDiagnostic(WebPartBaseAnalyzer.DIAGNOSTIC_ID);
            var classDeclaration = await baseTypeCodeFixHelper.GetDiagnosedClassDeclarationSyntax(WebPartBaseAnalyzer.DIAGNOSTIC_ID);
            if (classDeclaration == null)
            {
                return;
            }

            foreach (var classAndItsNamespace in WebPartBaseClasses)
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