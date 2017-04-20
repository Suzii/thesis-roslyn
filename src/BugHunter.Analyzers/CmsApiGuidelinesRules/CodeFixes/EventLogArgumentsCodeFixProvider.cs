using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventLogArgumentsCodeFixProvider)), Shared]
    public class EventLogArgumentsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(EventLogArgumentsAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var codeFixHelper = new MemberInvocationCodeFixHelper(context);
            var diagnostic = codeFixHelper.GetFirstDiagnostic();
            var invocationExpression = await codeFixHelper.GetDiagnosedInvocation();
            if (invocationExpression == null)
            {
                return;
            }

            var oldArgument = invocationExpression.ArgumentList.Arguments.First();
            var newArgumentName = GetNewMethodArgument(oldArgument);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newArgumentName),
                    createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldArgument, newArgumentName, "CMS.EventLog"),
                    equivalenceKey: nameof(EventLogArgumentsCodeFixProvider)),
                diagnostic);
        }

        private ArgumentSyntax GetNewMethodArgument(ArgumentSyntax oldArgument)
        {
            string newArgumentText = null;
            switch (oldArgument.Expression.ToString())
            {
                case "\"I\"":
                    newArgumentText = "EventType.INFORMATION";
                    break;
                case "\"W\"":
                    newArgumentText = "EventType.WARNING";
                    break;
                case "\"E\"":
                    newArgumentText = "EventType.ERROR";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(oldArgument));
            }

            return SyntaxFactory.Argument(SyntaxFactory.ParseExpression(newArgumentText));
        }
    }
}