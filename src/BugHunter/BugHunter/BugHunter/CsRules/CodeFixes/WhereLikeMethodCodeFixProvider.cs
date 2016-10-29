﻿using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugHunter.Core.ResourceBuilder;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
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
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            
            var memberAccessExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (memberAccessExpression == null)
            {
                return;
            }

            var containsMethodName = GetNewMethodName(BH1000PossibleFixes.WhereContains, memberAccessExpression);
            var startsWithMethodName = GetNewMethodName(BH1000PossibleFixes.WhereStartsWith, memberAccessExpression);
            var endsWithMethodName = GetNewMethodName(BH1000PossibleFixes.WhereEndsWith, memberAccessExpression);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(containsMethodName),
                    createChangedDocument: c => ReplaceWithDifferentMethodCall(context.Document, memberAccessExpression, c, containsMethodName),
                    equivalenceKey: "Contains()"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(startsWithMethodName),
                    createChangedDocument: c => ReplaceWithDifferentMethodCall(context.Document, memberAccessExpression, c, startsWithMethodName),
                    equivalenceKey: "StartsWith()"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(endsWithMethodName),
                    createChangedDocument: c => ReplaceWithDifferentMethodCall(context.Document, memberAccessExpression, c, endsWithMethodName),
                    equivalenceKey: "EndsWith()"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithDifferentMethodCall(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken cancellationToken, string newMethodName)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newMethodIdentifierName = SyntaxFactory.IdentifierName(newMethodName);
            var newMemberAccessExpression = memberAccessExpression.WithName(newMethodIdentifierName);

            var newRoot = root.ReplaceNode(memberAccessExpression, newMemberAccessExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private string GetNewMethodName(BH1000PossibleFixes forFix, MemberAccessExpressionSyntax currentMemberAccessExpression)
        {
            var isCurrentCallNegated = currentMemberAccessExpression.Name.Identifier.ToString().Contains("Not");

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