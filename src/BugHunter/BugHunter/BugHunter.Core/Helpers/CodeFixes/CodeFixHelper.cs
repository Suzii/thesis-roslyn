using System.Threading.Tasks;
using BugHunter.Core.Helpers.CodeFixes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Helpers.CodeFixes
{
    public class CodeFixHelper
    {
        protected readonly CodeFixContext Context;

        private SyntaxNode _documentRootCache;

        public CodeFixHelper(CodeFixContext context)
        {
            Context = context;
        }

        public async Task<Document> ReplaceExpressionWith(SyntaxNode oldNode, SyntaxNode newNode, params string[] namepsaceToBeReferenced)
        {
            var document = Context.Document;
            var root = await GetDocumentRoot();

            var formattedMemberAccess = newNode.WithTriviaFrom(oldNode);

            var newRoot = root.ReplaceNode(oldNode, formattedMemberAccess);

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