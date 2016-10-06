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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BH1001CodeFixProvider)), Shared]
    public class BH1001CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(BH1001EventLogArguments.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var invocationExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First();
            if (invocationExpression == null)
            {
                return;
            }

            var codeFixTitle = new LocalizableResourceString(nameof(CsResources.BH1000_CodeFix), CsResources.ResourceManager, typeof(CsResources)).ToString();
            var oldArgument = invocationExpression.ArgumentList.Arguments.First().Expression.ToString();
            var newArgumentName = GetNewMethodArgument(oldArgument);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: string.Format(codeFixTitle, oldArgument, newArgumentName),
                    createChangedDocument: c => ReplaceEventTypeArgument(context.Document, invocationExpression, c, newArgumentName),
                    equivalenceKey: "EventLog"),
                diagnostic);
        }

        private async Task<Document> ReplaceEventTypeArgument(Document document, InvocationExpressionSyntax invocationExpression, CancellationToken cancellationToken, string newArgumentName)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var eventLogExpression = SyntaxFactory.ParseExpression(newArgumentName);
            var newArgumentIdentifier = SyntaxFactory.Argument(eventLogExpression);
            var newArguments = SyntaxFactory.SeparatedList(new [] { newArgumentIdentifier}.Concat(invocationExpression.ArgumentList.Arguments.Skip(1)));
            var newArgumentList = SyntaxFactory.ArgumentList(newArguments);
            var newInvocationExpression = invocationExpression.WithArgumentList(newArgumentList);

            var newRoot = root.ReplaceNode(invocationExpression, newInvocationExpression);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private string GetNewMethodArgument(string oldArgument)
        {
            switch (oldArgument)
            {
                case "\"I\"":
                    return "CMS.EventLog.EventType.INFORMATION";
                case "\"W\"":
                    return "CMS.EventLog.EventType.WARNING";
                case "\"E\"":
                    return "CMS.EventLog.EventType.ERROR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldArgument));
            }
        }
    }
}