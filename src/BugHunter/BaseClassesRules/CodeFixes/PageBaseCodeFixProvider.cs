using System.Collections.Immutable;
using System.Composition;
using System.Linq;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PageBaseCodeFixProvider)), Shared]
    public class PageBaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(PageBaseAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        // TODO add more possibilities
        private static readonly ClassAndItsNamespace[] SuggestedBaseClasses =
        {
            new ClassAndItsNamespace { ClassNamespace = "CMS.UIControls", ClassName = "CMSAbstractEditablePage"},
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

            foreach (var classAndItsNamespace in SuggestedBaseClasses)
            {
                var newClassDeclaration = classDeclaration.WithBaseClass(classAndItsNamespace.ClassName);
                context.RegisterCodeFix(
                   CodeAction.Create(
                       title: $"Inherit from {classAndItsNamespace.ClassName} instead",
                       createChangedDocument: c => baseTypeCodeFixHelper.ReplaceExpressionWith(classDeclaration, newClassDeclaration, classAndItsNamespace.ClassNamespace),
                       equivalenceKey: nameof(PageBaseCodeFixProvider) + classAndItsNamespace.ClassName),
                   diagnostic);
            }
        }
    }
}