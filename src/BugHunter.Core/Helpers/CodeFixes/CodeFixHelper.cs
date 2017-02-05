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

            var formattedNewNode = newNode.WithTriviaFrom(oldNode);

            var newRoot = root.ReplaceNode(oldNode, formattedNewNode);

            if (namespacesToBeReferenced != null)
            {
                newRoot = UsingsHelper.EnsureUsings((CompilationUnitSyntax) newRoot, namespacesToBeReferenced);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        public Diagnostic GetFirstDiagnostic()
        {
            return Context.Diagnostics.First();
        }

        public Diagnostic GetFirstDiagnostic(string diagnosticId)
        {
            return Context.Diagnostics.FirstOrDefault(diagnostic => diagnostic.Id == diagnosticId);
        }

        public Location GetDiagnosticLocation()
        {
            return GetFirstDiagnostic().Location;
        }

        protected async Task<SyntaxNode> GetDocumentRoot()
        {
            return await Context.Document.GetSyntaxRootAsync(Context.CancellationToken).ConfigureAwait(false);
        }
    }
}