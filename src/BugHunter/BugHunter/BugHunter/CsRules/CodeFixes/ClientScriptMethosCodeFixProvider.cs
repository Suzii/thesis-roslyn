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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClientScriptMethosCodeFixProvider)), Shared]
    public class ClientScriptMethosCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ClientScriptMethodsAnalyzer.DIAGNOSTIC_ID);

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
            // Methods in script helper are named same as methods in ClientScriptManager
            // TODO different argument list
            var oldMemeberName = memberAccess.Name;
            var newMemberAccess = SyntaxFactory.ParseExpression($"ScriptHelper.{oldMemeberName}");
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, usingNamespace),
                    equivalenceKey: nameof(ClientScriptMethosCodeFixProvider)),
                diagnostic);
        }
    }
}