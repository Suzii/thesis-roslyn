using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    public class CodeFixHelper
    {
        protected readonly CodeFixContext Context;

        public CodeFixHelper(CodeFixContext context)
        {
            Context = context;
        }

        public async Task<Document> ReplaceExpressionWith(SyntaxNode oldNode, SyntaxNode newNode, params string[] namespacesToBeReferenced)
        {
            var document = Context.Document;
            var root = await GetDocumentRoot();

            var formattedMemberAccess = newNode.WithTriviaFrom(oldNode);

            var newRoot = root.ReplaceNode(oldNode, formattedMemberAccess);

            if (namespacesToBeReferenced != null)
            {
                newRoot = UsingsHelper.EnsureUsings((CompilationUnitSyntax) newRoot, namespacesToBeReferenced);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        public Location GetDiagnosticLocation()
        {
            return Context.Diagnostics.First().Location;
        }

        protected async Task<SyntaxNode> GetDocumentRoot()
        {
            return await Context.Document.GetSyntaxRootAsync(Context.CancellationToken).ConfigureAwait(false);
        }
    }
}