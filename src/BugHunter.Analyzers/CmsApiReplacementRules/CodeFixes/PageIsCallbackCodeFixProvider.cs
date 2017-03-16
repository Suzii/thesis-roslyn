using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PageIsCallbackCodeFixProvider)), Shared]
    public class PageIsCallbackCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(PageIsCallbackAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetDiagnosedMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var usingNamespace = "CMS.Helpers";
            var newMemberAccess = SyntaxFactory.ParseExpression("RequestHelper.IsCallback()");
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetReplaceWithMessage(newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, usingNamespace),
                    equivalenceKey: nameof(PageIsCallbackCodeFixProvider)),
                diagnostic);
        }
    }
}