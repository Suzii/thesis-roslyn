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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    /// <summary>
    /// Fixes "a".CompareTo("b") into string.Compare("a", "b", StringComparison.XY);
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringCompareToMethodCodeFixProvider)), Shared]
    public class StringCompareToMethodCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringCompareToMethodAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocation = await editor.GetDiagnosedInvocation();
            var memberAccess = invocation?.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                return;
            }

            if (IsChainedMemberAccesses(memberAccess))
            {
                // TODO
                // if this is the case, we cannot apply codefix directly as it is a static method call
                // new variable needs to be introduced to keep the result of the call
                // this variable would be later used in member access chain
                return;
            }

            var namespacesToBeReferenced = "System";
            var firstString = SyntaxFactory.Argument(memberAccess.Expression);
            var secondString = invocation.ArgumentList.Arguments.First();
            var staticInvocation = SyntaxFactory.ParseExpression("string.Compare()") as InvocationExpressionSyntax;

            foreach (var strignComparisonOption in StringComparisonOptions.GetAll())
            {
                var newInvocation = staticInvocation.AppendArguments(firstString, secondString).AppendArguments(strignComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation),
                        createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation, namespacesToBeReferenced),
                        equivalenceKey:
                        $"{nameof(StringCompareToMethodCodeFixProvider)}-{strignComparisonOption}"),
                    context.Diagnostics.First());
            }
        }

        private bool IsChainedMemberAccesses(MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Any();
        }
    }
}