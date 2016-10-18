using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using BugHunter.CsRules.Analyzers;
using BugHunter.Helpers.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.CsRules.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HttpSessionElementAccessCodeFixProvider)), Shared]
    public class HttpSessionElementAccessCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(HttpSessionElementAccessAnalyzer.DIAGNOSTIC_ID);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode oldNode = await GetSyntaxNodeToBeReplaced(context);
            SyntaxNode newNode;
            if (oldNode == null)
            {
                return;
            }


            var codeFixTitle = new LocalizableResourceString(nameof(CsResources.ApiReplacements_CodeFix), CsResources.ResourceManager, typeof(CsResources)).ToString();
            var diagnostic = context.Diagnostics.First();
            var usingNamespace = typeof(CMS.Helpers.SessionHelper).Namespace;
            var codeFixHelper = new CodeFixHelper(context);

            if (oldNode.Kind() == SyntaxKind.EqualsExpression)
            {
                // TODO
                newNode = SyntaxFactory.ParseExpression("SessionHelper.GetValue()");
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: string.Format(codeFixTitle, newNode),
                        createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldNode, newNode, usingNamespace),
                        equivalenceKey: "SessionHelper.GetValue"),
                    diagnostic);
            }
            else if (oldNode.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                // TODO
                newNode = SyntaxFactory.ParseExpression(@"SessionHelper.SetValue(""someKey"", ""someValue"")");
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: string.Format(codeFixTitle, newNode),
                        createChangedDocument: c => codeFixHelper.ReplaceExpressionWith(oldNode, newNode, usingNamespace),
                        equivalenceKey: "SessionHelper.SetValue"),
                    diagnostic);
            }
        }

        private async Task<SyntaxNode> GetSyntaxNodeToBeReplaced(CodeFixContext context)
        {
            throw new NotImplementedException();
            //var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            //var diagnostic = context.Diagnostics.First();
            //var diagnosticSpan = diagnostic.Location.SourceSpan;

            //var elementAccess = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ElementAccessExpressionSyntax>().First();
            //if (elementAccess == null)
            //{
            //    return null;
            //}

            //// many cases - EqualsValueClauseSyntax, SimpleAssignmentExpression...
            //var equalsNode = elementAccess.AncestorsAndSelf().OfType<EqualsValueClauseSyntax>().First();
            //if (equalsNode != null)
            //{
            //    return equalsNode;
            //}
        }
    }
}