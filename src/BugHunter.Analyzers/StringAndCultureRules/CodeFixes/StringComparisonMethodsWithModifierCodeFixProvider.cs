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

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringComparisonMethodsWithModifierCodeFixProvider)), Shared]
    public class StringComparisonMethodsWithModifierCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(
                StringEqualsMethodAnalyzer.DIAGNOSTIC_ID,
                StringStartAndEndsWithMethodsAnalyzer.DIAGNOSTIC_ID,
                StringIndexOfMethodsAnalyzer.DIAGNOSTIC_ID);

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

            foreach (var strignComparisonOption in StringComparisonOptions.GetAll())
            {
                var newInvocation = invocation.AppendArguments(strignComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation),
                        createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation, namespacesToBeReferenced),
                        equivalenceKey: $"{nameof(StringComparisonMethodsWithModifierCodeFixProvider)}-{strignComparisonOption}"),
                    context.Diagnostics.First());
            }
        }
    }
}