using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using BugHunter.StringMethodsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace BugHunter.StringMethodsRules.CodeFixes
{
    /// <summary>
    /// Fixes string.Equals("a", "b") into overload with StringComparison argument
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringEqualsStaticMethodCodeFixProvider)), Shared]
    public class StringEqualsStaticMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringEqualsStaticMethodAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocation = await editor.GetDiagnosedInvocation();
            if (invocation == null)
            {
                return;
            }

            var namespacesToBeReferenced = "System";
            
            foreach (var strignComparisonOption in StringComparisonOptions.GetAll())
            {
                var newInvocation = invocation.AppendArguments(strignComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessageBuilder.GetMessage(newInvocation),
                        createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation, namespacesToBeReferenced),
                        equivalenceKey: $"{nameof(StringEqualsStaticMethodCodeFixProvider)}-{strignComparisonOption}"),
                    context.Diagnostics.First());
            }
        }
    }
}