using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringManipultionMethodsCodeFixProvider)), Shared]
    public class StringManipultionMethodsCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(StringManipulationMethodsAnalyzer.DIAGNOSTIC_ID);

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

            var diagnostic = context.Diagnostics.First();
            var methodName = invocation.Expression.ToString();
            var newInvocation1 = SyntaxFactory.ParseExpression($"{methodName}Invariant()");
            var newInvocation2 = SyntaxFactory.ParseExpression($"{methodName}(CultureInfo.CurrentCulture)");

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation1),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation1),
                    equivalenceKey: $"{nameof(StringManipultionMethodsCodeFixProvider)}-InvariantCulture"),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newInvocation2),
                    createChangedDocument: c => editor.ReplaceExpressionWith(invocation, newInvocation2),
                    equivalenceKey: $"{nameof(StringManipultionMethodsCodeFixProvider)}-CurrentCulture"),
                diagnostic);
        }
    }
}