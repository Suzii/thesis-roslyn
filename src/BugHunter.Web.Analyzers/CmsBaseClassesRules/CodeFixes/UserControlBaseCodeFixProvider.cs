﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Models;
using BugHunter.Core.ResourceBuilder;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.Web.Analyzers.CmsBaseClassesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UserControlBaseCodeFixProvider)), Shared]
    public class UserControlBaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UserControlBaseAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        private static readonly ClassAndItsNamespace[] SuggestedBaseClasses =
        {
            new ClassAndItsNamespace { ClassNamespace = "CMS.UIControls", ClassName = "CMSUserControl"},
            new ClassAndItsNamespace { ClassNamespace = "CMS.Base.Web.UI", ClassName = "AbstractUserControl"},
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
                       title: CodeFixMessageBuilder.GetInheritFromMessage(classAndItsNamespace.ClassName),
                       createChangedDocument: c => baseTypeCodeFixHelper.ReplaceExpressionWith(classDeclaration, newClassDeclaration, classAndItsNamespace.ClassNamespace),
                       equivalenceKey: nameof(UserControlBaseCodeFixProvider) + classAndItsNamespace.ClassName),
                   diagnostic);
            }
        }
    }
}