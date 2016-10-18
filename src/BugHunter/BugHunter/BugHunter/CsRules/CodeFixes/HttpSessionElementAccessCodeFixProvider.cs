//using System.Collections.Immutable;
//using System.Composition;
//using System.Linq;
//using System.Threading.Tasks;
//using BugHunter.CsRules.Analyzers;
//using BugHunter.Helpers.CodeFixes;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CodeActions;
//using Microsoft.CodeAnalysis.CodeFixes;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace BugHunter.CsRules.CodeFixes
//{
//    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpSessionElementAccessCodeFixProvider)), Shared]
//    public class HttpSessionElementAccessCodeFixProvider : CodeFixProvider
//    {
//        public sealed override ImmutableArray<string> FixableDiagnosticIds
//            => ImmutableArray.Create(HttpSessionElementAccessAnalyzer.DIAGNOSTIC_ID);

//        public sealed override FixAllProvider GetFixAllProvider()
//        {
//            return WellKnownFixAllProviders.BatchFixer;
//        }

//        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
//        {
//            ExpressionSyntax oldExpression = await GetSyntaxNodeToBeReplaced(context);
//            ExpressionSyntax newExpression;
//            if (oldExpression == null)
//            {
//                return;
//            }

//            if (oldExpression.Kind() == SyntaxKind.EqualsExpression)
//            {
//                // TODO
//                newExpression = SyntaxFactory.ParseExpression("SessionHelper.GetValue()");
//            }
//            else if(oldExpression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
//            {
//                // TODO
//                oldExpression = SyntaxFactory.ParseExpression(@"SessionHelper.SetValue(""someKey"", ""someValue"")");
//            }

//            var codeFixTitle = new LocalizableResourceString(nameof(CsResources.ApiReplacements_CodeFix), CsResources.ResourceManager, typeof(CsResources)).ToString();
//            var usingNamespace = typeof(CMS.Helpers.SessionHelper).Namespace;

//            context.RegisterCodeFix(
//                CodeAction.Create(
//                    title: string.Format(codeFixTitle, newMemberAccess),
//                    createChangedDocument: c => editor.ReplaceWithHelper(memberAccess, newMemberAccess, usingNamespace),
//                    equivalenceKey: "SessionHelper.GetValue"),
//                diagnostic);

//            context.RegisterCodeFix(
//                CodeAction.Create(
//                    title: string.Format(codeFixTitle, newMemberAccess),
//                    createChangedDocument: c => editor.ReplaceWithHelper(memberAccess, newMemberAccess, usingNamespace),
//                    equivalenceKey: "SessionHelper.SetValue"),
//                diagnostic);
//        }

//        private async Task<ExpressionSyntax> GetSyntaxNodeToBeReplaced(CodeFixContext context)
//        {
//            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
//            var diagnostic = context.Diagnostics.First();
//            var diagnosticSpan = diagnostic.Location.SourceSpan;

//            var elementAccess = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ElementAccessExpressionSyntax>().First();
//            if (elementAccess == null)
//            {
//                return;
//            }

//            // we should be looking for ElementAccessExpressionSyntax, but if it is enclosed within more complicated expression e.g. HttpContext.Session["something"]
//            // we want to replace whole MemberAccessExpressionSyntax, including HttpContext
//            ExpressionSyntax oldExpression = elementAccess.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
//            oldExpression = oldExpression ?? elementAccess;

//        }
//    }
//}