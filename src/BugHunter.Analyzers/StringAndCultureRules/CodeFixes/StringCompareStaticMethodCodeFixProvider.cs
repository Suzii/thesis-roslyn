using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    /// <summary>
    /// Fixes string.Compare("a", "b") into overload with StringComparison argument
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringCompareStaticMethodCodeFixProvider)), Shared]
    public class StringCompareStaticMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringCompareStaticMethodAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocation = await editor.GetDiagnosedInvocation();
            if (invocation == null)
            {
                return;
            }

            const string namespacesToBeReferenced = "System";
            var tempInvocation = invocation;
            var stringComparisonOptions = StringComparisonOptions.GetAll();

            // if it was diagnosed and overload with three or six arguments is called it must be:
            // public static int Compare(string strA, string strB, bool ignoreCase)
            // OR
            // public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase)
            // therefore we will apply only suitable fixes
            var originalArguments = invocation.ArgumentList.Arguments;
            if (originalArguments.Count == 3 || originalArguments.Count == 6)
            {
                var ignoreCaseAttribute = originalArguments.Last().Expression.ToString();
                bool ignoreCase;

                if (!bool.TryParse(ignoreCaseAttribute, out ignoreCase))
                {
                    // could not detect value of 'ignoreCase' argument -> no codefix provided
                    return;
                }

                tempInvocation = tempInvocation.WithArgumentList(SyntaxFactory.ArgumentList(originalArguments.RemoveAt(originalArguments.Count - 1)));
                stringComparisonOptions = ignoreCase
                    ? StringComparisonOptions.GetCaseInsensitive()
                    : StringComparisonOptions.GetCaseSensitive();
            }

            foreach (var stringComparisonOption in stringComparisonOptions)
            {
                var newInvocation = tempInvocation.AppendArguments(stringComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation),
                        createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation, namespacesToBeReferenced),
                        equivalenceKey: $"{nameof(StringCompareStaticMethodCodeFixProvider)}-{stringComparisonOption}"),
                    context.Diagnostics.First());
            }
        }
    }
}