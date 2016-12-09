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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpSessionElementAccessGetCodeFixProvider)), Shared]
    public class HttpSessionElementAccessGetCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpSessionElementAccessAnalyzer.DIAGNOSTIC_ID_GET);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var elementAccess = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ElementAccessExpressionSyntax>().FirstOrDefault();
            if (elementAccess == null)
            {
                return;
            }

            var usingNamespace = "CMS.Helpers";
            var codeFixHelper = new CodeFixHelper(context);
            var sessionKey = GetElementAccessKey(elementAccess);

            SyntaxNode oldNode = elementAccess;
            SyntaxNode newNode = SyntaxFactory.ParseExpression($@"SessionHelper.GetValue({sessionKey})");

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixMessageBuilder.GetMessage(newNode),
                    createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldNode, newNode, usingNamespace),
                    equivalenceKey: nameof(HttpSessionElementAccessGetCodeFixProvider)),
                diagnostic);
        }

        private ArgumentSyntax GetElementAccessKey(ElementAccessExpressionSyntax elementAccess)
        {
            return elementAccess.ArgumentList.Arguments.First();
        }
    }
}