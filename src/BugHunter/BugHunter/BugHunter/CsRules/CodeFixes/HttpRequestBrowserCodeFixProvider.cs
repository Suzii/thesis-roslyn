using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using BugHunter.CsRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpRequestBrowserCodeFixProvider)), Shared]
    public class HttpRequestBrowserCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpRequestBrowserAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetDiagnosedMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var newMemberAccess = SyntaxFactory.ParseExpression("BrowserHelper.GetBrowser()");
            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, usingNamespace),
                    equivalenceKey: nameof(HttpRequestBrowserCodeFixProvider)),
                diagnostic);
        }
    }
}