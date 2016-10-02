using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    internal enum PossibleFixes
    {
        WhereContains,
        WhereStartsWith,
        WhereEndsWith
    }

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BH1000CodeFixProvider)), Shared]
    public class BH1000CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(BH1000MethodWhereLikeShouldNotBeUsed.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            
            var memberAccessExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
            if (memberAccessExpression == null)
            {
                return;
            }

            var containsMethodName = GetNewMethodName(PossibleFixes.WhereContains, memberAccessExpression);
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Replace with {containsMethodName}()",
                    createChangedDocument: c => ReplaceWithWithDifferentMethodCall(context.Document, memberAccessExpression, c, containsMethodName),
                    equivalenceKey: "Contains()"),
                diagnostic);

            var startsWithMethodName = GetNewMethodName(PossibleFixes.WhereStartsWith, memberAccessExpression);
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Replace with {startsWithMethodName}()",
                    createChangedDocument: c => ReplaceWithWithDifferentMethodCall(context.Document, memberAccessExpression, c, startsWithMethodName),
                    equivalenceKey: "StartsWith()"),
                diagnostic);

            var endsWithMethodName = GetNewMethodName(PossibleFixes.WhereEndsWith, memberAccessExpression);
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: $"Replace with {endsWithMethodName}()",
                    createChangedDocument: c => ReplaceWithWithDifferentMethodCall(context.Document, memberAccessExpression, c, endsWithMethodName),
                    equivalenceKey: "EndsWith()"),
                diagnostic);
        }

        private async Task<Document> ReplaceWithWithDifferentMethodCall(Document document, MemberAccessExpressionSyntax memberAccessExpression, CancellationToken cancellationToken, string newMethodName)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newMethodIdentifierName = SyntaxFactory.IdentifierName(newMethodName);
            var newMemberAccessExpression = memberAccessExpression.WithName(newMethodIdentifierName);

            var newRoot = root.ReplaceNode(memberAccessExpression, newMemberAccessExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private string GetNewMethodName(PossibleFixes forFix, MemberAccessExpressionSyntax currentMemberAccessExpression)
        {
            var isCurrentCallNegated = currentMemberAccessExpression.Name.Identifier.ToString().Contains("Not");

            switch (forFix)
            {
                case PossibleFixes.WhereContains:
                    return isCurrentCallNegated ? "WhereNotContains" : "WhereContains";
                case PossibleFixes.WhereStartsWith:
                    return isCurrentCallNegated ? "WhereNotStartsWith" : "WhereStartsWith";
                case PossibleFixes.WhereEndsWith:
                    return isCurrentCallNegated ? "WhereNotEndsWith" : "WhereEndsWith";
                default:
                    throw new ArgumentOutOfRangeException(nameof(forFix));
            }
        }
    }
}