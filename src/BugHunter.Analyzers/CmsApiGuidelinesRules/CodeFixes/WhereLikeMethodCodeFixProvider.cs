using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhereLikeMethodCodeFixProvider)), Shared]
    public class WhereLikeMethodCodeFixProvider : CodeFixProvider
    {
        internal enum BH1000PossibleFixes
        {
            WhereContains,
            WhereStartsWith,
            WhereEndsWith
        }

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(WhereLikeMethodAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var codeFixHelper = new MemberInvocationCodeFixHelper(context);
            var invocationExpression = await codeFixHelper.GetDiagnosedInvocation();
            SimpleNameSyntax methodNameNode;
            if (invocationExpression == null || !invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return;
            }



            var containsMethodName = GetNewMethodName(BH1000PossibleFixes.WhereContains, methodNameNode);
            var startsWithMethodName = GetNewMethodName(BH1000PossibleFixes.WhereStartsWith, methodNameNode);
            var endsWithMethodName = GetNewMethodName(BH1000PossibleFixes.WhereEndsWith, methodNameNode);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(containsMethodName),
                    createChangedDocument: c => ReplaceWithDifferentMethodCall(context.Document, methodNameNode, c, containsMethodName),
                    equivalenceKey: "Contains()"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(startsWithMethodName),
                    createChangedDocument: c => ReplaceWithDifferentMethodCall(context.Document, methodNameNode, c, startsWithMethodName),
                    equivalenceKey: "StartsWith()"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(endsWithMethodName),
                    createChangedDocument: c => ReplaceWithDifferentMethodCall(context.Document, methodNameNode, c, endsWithMethodName),
                    equivalenceKey: "EndsWith()"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithDifferentMethodCall(Document document, SimpleNameSyntax oldMethodNameNode, CancellationToken cancellationToken, string newMethodName)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newMethodIdentifierName = SyntaxFactory.IdentifierName(newMethodName);
            //var newMemberAccessExpression = memberAccessExpression.WithName(newMethodIdentifierName);

            var newRoot = root.ReplaceNode(oldMethodNameNode, newMethodIdentifierName);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private string GetNewMethodName(BH1000PossibleFixes forFix, SimpleNameSyntax currentMethodName)
        {
            var isCurrentCallNegated = currentMethodName.Identifier.ToString().Contains("Not");

            switch (forFix)
            {
                case BH1000PossibleFixes.WhereContains:
                    return isCurrentCallNegated ? "WhereNotContains" : "WhereContains";
                case BH1000PossibleFixes.WhereStartsWith:
                    return isCurrentCallNegated ? "WhereNotStartsWith" : "WhereStartsWith";
                case BH1000PossibleFixes.WhereEndsWith:
                    return isCurrentCallNegated ? "WhereNotEndsWith" : "WhereEndsWith";
                default:
                    throw new ArgumentOutOfRangeException(nameof(forFix));
            }
        }
    }
}