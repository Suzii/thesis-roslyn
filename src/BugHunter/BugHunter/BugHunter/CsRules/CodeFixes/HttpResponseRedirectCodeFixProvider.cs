﻿using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpResponseRedirectCodeFixProvider)), Shared]
    public class HttpResponseRedirectCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpResponseRedirectAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetDiagnosedMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var codeFix1 = SyntaxFactory.ParseExpression("UrlHelper.Redirect");
            var codeFix2 = SyntaxFactory.ParseExpression("UrlHelper.LocalRedirect");

            var message1 = $"{CodeFixMessageBuilder.GetMessage(codeFix2)} {CsResources.RedirectCodeFixLocal}";
            var message2 = $"{CodeFixMessageBuilder.GetMessage(codeFix2)} {CsResources.RedirectCodeFixExternal}";

            var diagnostic = context.Diagnostics.First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: message1,
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, codeFix1, usingNamespace),
                    equivalenceKey: nameof(HttpResponseRedirectCodeFixProvider)),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: message2,
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, codeFix2, usingNamespace),
                    equivalenceKey: nameof(HttpResponseRedirectCodeFixProvider) + "Local"),
                    diagnostic);
        }
    }
}