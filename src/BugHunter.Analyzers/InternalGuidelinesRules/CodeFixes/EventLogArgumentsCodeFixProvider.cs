using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugHunter.Analyzers.InternalGuidelinesRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.InternalGuidelinesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventLogArgumentsCodeFixProvider)), Shared]
    public class EventLogArgumentsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(EventLogArgumentsAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var invocationExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();
            if (invocationExpression == null)
            {
                return;
            }

            var oldArgument = invocationExpression.ArgumentList.Arguments.First().Expression.ToString();
            var newArgumentName = GetNewMethodArgument(oldArgument);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newArgumentName),
                    createChangedDocument: c => ReplaceEventTypeArgument(context.Document, invocationExpression, c, newArgumentName),
                    equivalenceKey: nameof(EventLogArgumentsCodeFixProvider)),
                diagnostic);
        }

        private async Task<Document> ReplaceEventTypeArgument(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken, string newArgumentName)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var eventLogExpression = SyntaxFactory.ParseExpression(newArgumentName);
            var newArgumentIdentifier = SyntaxFactory.Argument(eventLogExpression);
            var newArguments = SyntaxFactory.SeparatedList(new [] { newArgumentIdentifier }.Concat(invocationExpression.ArgumentList.Arguments.Skip(1)));
            var newArgumentList = SyntaxFactory.ArgumentList(newArguments);
            var newInvocationExpression = invocationExpression.WithArgumentList(newArgumentList);

            var newRoot = root.ReplaceNode(invocationExpression, newInvocationExpression);
            newRoot = UsingsHelper.EnsureUsings((CompilationUnitSyntax)newRoot, GetNamespaceNameToBeReferenced());
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private string GetNewMethodArgument(string oldArgument)
        {
            switch (oldArgument)
            {
                case "\"I\"":
                    return "EventType.INFORMATION";
                case "\"W\"":
                    return "EventType.WARNING";
                case "\"E\"":
                    return "EventType.ERROR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldArgument));
            }
        }

        private string GetNamespaceNameToBeReferenced()
        {
            return "CMS.EventLog";
        }
    }
}