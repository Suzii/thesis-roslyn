using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
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
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberInvocationCodeFixHelper(context);
            var invocation = await editor.GetDiagnosedInvocation();
            if (!IsFixable(invocation))
            {
                return;
            }

            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
            if (IsChainedMemberAccesses(memberAccess))
            {
                // TODO
                // if this is the case, we cannot apply codefix directly as it is a static method call
                // new variable needs to be introduced to keep the result of the call
                // this variable would be later used in member access chain
                return;
            }

            const string namespacesToBeReferenced = "System";
            var firstString = SyntaxFactory.Argument(memberAccess.Expression);
            var secondString = invocation.ArgumentList.Arguments.First();
            var staticInvocation = SyntaxFactory.ParseExpression("string.Compare()") as InvocationExpressionSyntax;

            foreach (var stringComparisonOption in StringComparisonOptions.GetAll())
            {
                var newInvocation = staticInvocation.AppendArguments(firstString, secondString).AppendArguments(stringComparisonOption);

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeFixMessagesProvider.GetReplaceWithMessage(newInvocation),
                        createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation, namespacesToBeReferenced),
                        equivalenceKey:
                        $"{nameof(StringCompareToMethodCodeFixProvider)}-{stringComparisonOption}"),
                    context.Diagnostics.First());
            }
        }

        private static bool IsFixable(InvocationExpressionSyntax invocation)
        {
            return invocation != null && 
                   invocation.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression) &&
                   !invocation.Parent.IsKind(SyntaxKind.ConditionalAccessExpression);
        }

        private static bool IsChainedMemberAccesses(MemberAccessExpressionSyntax memberAccess)
        {
            return memberAccess.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Any();
        }
    }
}