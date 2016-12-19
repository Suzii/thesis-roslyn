using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using BugHunter.StringMethodsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.StringMethodsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringEqualsMethodCodeFixProvider)), Shared]
    public class StringEqualsMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringEqualsMethodAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new CodeFixHelper(context);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var root = await context.Document.GetSyntaxRootAsync();
            var invocation = root.FindNode(diagnosticSpan, true).FirstAncestorOrSelf<InvocationExpressionSyntax>();

            if (invocation == null)
            {
                return;
            }

            foreach (var strignComparisonOption in StringComparisonOptions())
            {
                var newInvocation = ConstructNewInvocation(strignComparisonOption, invocation);

                context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newInvocation),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation),
                    equivalenceKey: $"{nameof(StringEqualsMethodCodeFixProvider)}-{strignComparisonOption}"),
                diagnostic);
            }
        }

        private static InvocationExpressionSyntax ConstructNewInvocation(string strignComparisonOption, InvocationExpressionSyntax invocation)
        {
            var addedArgument = SyntaxFactory.Argument(SyntaxFactory.ParseExpression(strignComparisonOption));
            var newMethodArguments = invocation.ArgumentList.Arguments.Add(addedArgument);
            var newArgumentList = SyntaxFactory.ArgumentList(newMethodArguments);
            var newInvocation = invocation.WithArgumentList(newArgumentList);

            return newInvocation;
        }

        private IEnumerable<string> StringComparisonOptions()
        {
            yield return "StringComparison.CurrentCulture";
            yield return "StringComparison.CurrentCultureIgnoreCase";
            yield return "StringComparison.InvariantCulture";
            yield return "StringComparison.InvariantCultureIgnoreCase";
        }
    }
}