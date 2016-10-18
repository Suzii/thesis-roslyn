using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Helpers.CodeFixes
{
    internal class CodeFixHelper
    {
        protected readonly CodeFixContext Context;

        private SyntaxNode _documentRootCache;

        public CodeFixHelper(CodeFixContext context)
        {
            Context = context;
        }

        public async Task<Document> ReplaceExpressionWith(ExpressionSyntax oldExpression, ExpressionSyntax newExpression, params string[] namepsaceToBeReferenced)
        {
            var document = Context.Document;
            var root = await GetDocumentRoot();

            var formattedMemberAccess = newExpression.WithTriviaFrom(oldExpression);

            var newRoot = root.ReplaceNode(oldExpression, formattedMemberAccess);

            if (namepsaceToBeReferenced != null)
            {
                newRoot = UsingsHelper.EnsureUsings((CompilationUnitSyntax) newRoot, namepsaceToBeReferenced);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        protected async Task<SyntaxNode> GetDocumentRoot()
        {
            if (_documentRootCache == null)
            {
                _documentRootCache = await Context.Document.GetSyntaxRootAsync(Context.CancellationToken).ConfigureAwait(false);
            }

            return _documentRootCache;
        }
    }
}