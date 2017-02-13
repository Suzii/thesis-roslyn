using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.WebInternalGuidelinesRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.WebInternalGuidelinesRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ValidationHelperGetCodeFixProvider)), Shared]
    public class ValidationHelperGetCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ValidationHelperGetAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetDiagnosedMemberAccess();

            var enclosingInvocation = memberAccess?.FirstAncestorOrSelf<InvocationExpressionSyntax>();
            if (enclosingInvocation == null)
            {
                return;
            }

            var argumentsToSustain = enclosingInvocation.ArgumentList.Arguments.Take(2).ToArray();
            var newInvocation = SyntaxFactory.ParseExpression($"ValidationHelper.{memberAccess.Name}System()") as InvocationExpressionSyntax;
            newInvocation = newInvocation.AppendArguments(argumentsToSustain);

            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newInvocation),
                    createChangedDocument: c => editor.ReplaceExpressionWith(enclosingInvocation, newInvocation, "CMS.Helpers"),
                    equivalenceKey: nameof(ValidationHelperGetCodeFixProvider)),
                diagnostic);
        }
    }
}