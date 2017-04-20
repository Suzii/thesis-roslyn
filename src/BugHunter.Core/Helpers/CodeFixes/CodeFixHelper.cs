using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    /// <summary>
    /// Class handling common tasks related to code fixes. Always instantiated with concrete <see cref="CodeFixContext"/> instance.
    /// </summary>
    public class CodeFixHelper
    {
        /// <summary>
        /// Context of the code fix
        /// </summary>
        protected readonly CodeFixContext Context;

        public CodeFixHelper(CodeFixContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Replaces <param name="oldNode"></param> with <param name="newNode"></param> in document associated to current codefix context and adds using directive if provided
        /// </summary>
        /// <param name="oldNode">Node to be replaced</param>
        /// <param name="newNode">Node to be used as replacement</param>
        /// <param name="namespacesToBeReferenced">Optional namespaces to be referenced in document if new node requires it</param>
        /// <returns>Changed document with replaced nodes</returns>
        public async Task<Document> ReplaceExpressionWith(SyntaxNode oldNode, SyntaxNode newNode, params string[] namespacesToBeReferenced)
        {
            return await ApplyRootModification((oldRoot) => oldRoot.ReplaceNode(oldNode, newNode.WithTriviaFrom(oldNode)), namespacesToBeReferenced);
        }

        /// <summary>
        /// Applies the <param name="rootModificationFunc"></param> on root of the document associated with current code fix context and adds usings directives if priveded
        /// </summary>
        /// <param name="rootModificationFunc">Root modification function to be applied</param>
        /// <param name="namespacesToBeReferenced">Optional namespaces to be referenced in document if new version of the document requires it</param>
        /// <returns>Changed document with applied function</returns>
        public async Task<Document> ApplyRootModification(Func<CompilationUnitSyntax, CompilationUnitSyntax> rootModificationFunc, params string[] namespacesToBeReferenced)
        {
            var document = Context.Document;
            var root = await GetDocumentRoot();

            var newRoot = rootModificationFunc(root);

            if (namespacesToBeReferenced != null)
            {
                newRoot = UsingsHelper.EnsureUsings(newRoot, namespacesToBeReferenced);
            }

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        /// <summary>
        /// Returns first <see cref="Diagnostic"/> associated with current code fix context
        /// </summary>
        /// <returns>First diagnostic in current code fix context</returns>
        public Diagnostic GetFirstDiagnostic()
        {
            return Context.Diagnostics.First();
        }

        /// <summary>
        /// Returns first <see cref="Diagnostic"/> associated with current code fix context that has one if id from <param name="diagnosticIds"></param>
        /// </summary>
        /// <returns>First diagnostic in current code fix context with matching id</returns>
        public Diagnostic GetFirstDiagnostic(params string[] diagnosticIds)
        {
            return Context.Diagnostics.FirstOrDefault(diagnostic => diagnosticIds.Contains(diagnostic.Id));
        }

        /// <summary>
        /// Returns the <see cref="Location"/> of first <see cref="Diagnostic"/> associated with current code fix context
        /// </summary>
        /// <returns>Location of firs diagnostic in current code fix context</returns>
        public Location GetDiagnosticLocation()
        {
            return GetFirstDiagnostic().Location;
        }

        /// <summary>
        /// Returns the root of document associated with current code fix context
        /// </summary>
        /// <returns>Root of the document</returns>
        protected async Task<CompilationUnitSyntax> GetDocumentRoot()
        {
            return (CompilationUnitSyntax) await Context.Document.GetSyntaxRootAsync(Context.CancellationToken).ConfigureAwait(false);
        }
    }
}