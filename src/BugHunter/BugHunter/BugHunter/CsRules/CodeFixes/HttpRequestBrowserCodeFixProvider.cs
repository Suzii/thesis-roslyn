﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.CsRules.Analyzers;
using BugHunter.Helpers.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpRequestBrowserCodeFixProvider)), Shared]
    public class HttpRequestBrowserCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpRequestBrowserAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetOutterMostMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var codeFixTitle = new LocalizableResourceString(nameof(CsResources.ApiReplacements_CodeFix), CsResources.ResourceManager, typeof(CsResources)).ToString();
            var usingNamespace = typeof(CMS.Helpers.BrowserHelper).Namespace;
            var newMemberAccess = SyntaxFactory.ParseExpression("BrowserHelper.GetBrowser()");
            var diagnostic = context.Diagnostics.First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(codeFixTitle, newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, usingNamespace),
                    equivalenceKey: nameof(HttpRequestBrowserCodeFixProvider)),
                diagnostic);
        }
    }
}