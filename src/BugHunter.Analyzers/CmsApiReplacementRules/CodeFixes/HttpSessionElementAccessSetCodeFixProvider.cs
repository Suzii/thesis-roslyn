using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Core.Helpers.CodeFixes;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpSessionElementAccessSetCodeFixProvider)), Shared]
    public class HttpSessionElementAccessSetCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpSessionElementAccessAnalyzer.DIAGNOSTIC_ID_SET);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var diagnosedNode = root.FindToken(diagnosticSpan.Start).Parent;
            var elementAccess = diagnosedNode.AncestorsAndSelf().OfType<ElementAccessExpressionSyntax>().FirstOrDefault() ?? diagnosedNode.DescendantNodesAndSelf().OfType<ElementAccessExpressionSyntax>().FirstOrDefault();
            if (elementAccess == null)
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var codeFixHelper = new CodeFixHelper(context);

            var assignmentExpression = elementAccess.FirstAncestorOrSelf<AssignmentExpressionSyntax>();
            var sessionKey = GetElementAccessKey(elementAccess);
            var valueToBeAssigned = assignmentExpression.Right;
            
            SyntaxNode oldNode = assignmentExpression;
            SyntaxNode newNode = SyntaxFactory.ParseExpression($@"SessionHelper.SetValue({sessionKey}, {valueToBeAssigned})");

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessagesProvider.GetReplaceWithMessage(newNode),
                    createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldNode, newNode, usingNamespace),
                    equivalenceKey: nameof(HttpSessionElementAccessSetCodeFixProvider)),
                diagnostic);
        }

        private ArgumentSyntax GetElementAccessKey(ElementAccessExpressionSyntax elementAccess)
        {
            return elementAccess.ArgumentList.Arguments.First();
        }
    }
}