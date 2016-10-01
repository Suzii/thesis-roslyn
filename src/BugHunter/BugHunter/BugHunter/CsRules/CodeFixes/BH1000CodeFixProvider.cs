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
using Microsoft.CodeAnalysis.Rename;

namespace BugHunter.CsRules.CodeFixes
{
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

            // Find the type declaration identified by the diagnostic.
            var invocationExpression =
                root.FindToken(diagnosticSpan.Start)
                    .Parent.AncestorsAndSelf()
                    .OfType<MemberAccessExpressionSyntax>()
                    .First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with WhereContains()",
                    createChangedDocument: c => ReplaceWithContainsMethod(context.Document, invocationExpression, c),
                    equivalenceKey: "Replace with WhereContains()"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with WhereStartsWith()",
                    createChangedDocument: c => ReplaceWithStartsWithMethod(context.Document, invocationExpression, c),
                    equivalenceKey: "Replace with WhereStartsWith()"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Replace with WhereEndsWith()",
                    createChangedDocument: c => ReplaceWithEndsWithMethod(context.Document, invocationExpression, c),
                    equivalenceKey: "Replace with WhereEndsWith()"),
                diagnostic);
        }

        // TODO finish this once references versions conflicts are resolved
        private async Task<Document> ReplaceWithContainsMethod(Document document, MemberAccessExpressionSyntax memberAccessExpressionSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            
            var invocationExpr = memberAccessExpressionSyntax.Parent as InvocationExpressionSyntax;
            if (invocationExpr == null)
            {
                throw new ArgumentException(nameof(memberAccessExpressionSyntax));
            }


            var contains = SyntaxFactory.IdentifierName("WhereContains");
            var expr = SyntaxFactory.ExpressionStatement(contains);
            var newInvocationExpression = invocationExpr.WithExpression(expr.Expression);

            //var newCall =
            //    SyntaxFactory.ParseStatement(
            //        $"{memberAccessExpressionSyntax.Name}{memberAccessExpressionSyntax.OperatorToken}Contains({string.Join(", ", invocationExpr.ArgumentList)}");


            // TODO
            var newRoot = root.ReplaceNode(invocationExpr, newInvocationExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private Task<Document> ReplaceWithEndsWithMethod(Document document, MemberAccessExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private Task<Document> ReplaceWithStartsWithMethod(Document document, MemberAccessExpressionSyntax invocationExpression, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}