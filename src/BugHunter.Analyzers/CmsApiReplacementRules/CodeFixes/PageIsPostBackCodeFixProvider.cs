using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PageIsPostBackCodeFixProvider)), Shared]
    public class PageIsPostBackCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(PageIsPostBackAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            if (!diagnostic.IsMarkedAsSimpleMemberAccess())
            {
                // no codefix for conditional accesses
                return;
            }

            var editor = new MemberAccessCodeFixHelper(context);
            var memberAccess = await editor.GetDiagnosedMemberAccess();

            if (memberAccess == null)
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var newMemberAccess = SyntaxFactory.ParseExpression("RequestHelper.IsPostBack()");
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newMemberAccess),
                    createChangedDocument: c => editor.ReplaceExpressionWith(memberAccess, newMemberAccess, c, usingNamespace),
                    equivalenceKey: nameof(PageIsPostBackCodeFixProvider)),
                diagnostic);
        }
    }
}