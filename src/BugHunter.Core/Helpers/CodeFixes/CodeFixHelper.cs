// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
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
        /// Initializes a new instance of the <see cref="CodeFixHelper"/> class.
        /// </summary>
        /// <param name="context">Context of the code fix</param>
        public CodeFixHelper(CodeFixContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets context of the code fix
        /// </summary>
        protected CodeFixContext Context { get; }

        /// <summary>
        /// Replaces <paramref name="oldNode" /> with <paramref name="newNode" /> in document associated to current codefix context and adds using directive if provided
        /// </summary>
        /// <param name="oldNode">Node to be replaced</param>
        /// <param name="newNode">Node to be used as replacement</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="namespacesToBeReferenced">Optional namespaces to be referenced in document if new node requires it</param>
        /// <returns>Changed document with replaced nodes</returns>
        public async Task<Document> ReplaceExpressionWith(SyntaxNode oldNode, SyntaxNode newNode, CancellationToken cancellationToken, params string[] namespacesToBeReferenced)
        {
            return await ApplyRootModification((oldRoot) => oldRoot.ReplaceNode(oldNode, newNode.WithTriviaFrom(oldNode)), cancellationToken, namespacesToBeReferenced);
        }

        /// <summary>
        /// Applies the <paramref name="rootModificationFunc" /> on root of the document associated with current code fix context and adds usings directives if priveded
        /// </summary>
        /// <param name="rootModificationFunc">Root modification function to be applied</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="namespacesToBeReferenced">Optional namespaces to be referenced in document if new version of the document requires it</param>
        /// <returns>Changed document with applied function</returns>
        public async Task<Document> ApplyRootModification(Func<CompilationUnitSyntax, CompilationUnitSyntax> rootModificationFunc, CancellationToken cancellationToken, params string[] namespacesToBeReferenced)
        {
            var document = Context.Document;
            var root = await GetDocumentRoot(cancellationToken);

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
        /// Returns first <see cref="Diagnostic"/> associated with current code fix context that has one if id from <paramref name="diagnosticIds" />
        /// </summary>
        /// <param name="diagnosticIds">Possible IDs of diagnostic that should be returned</param>
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
        /// Returns the root of document associated with current code fix context.
        /// </summary>
        /// <returns>Root of the document</returns>
        protected async Task<CompilationUnitSyntax> GetDocumentRoot()
        {
            return (CompilationUnitSyntax)await Context.Document.GetSyntaxRootAsync(Context.CancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Returns the root of document associated with current code fix context.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Root of the document</returns>
        protected async Task<CompilationUnitSyntax> GetDocumentRoot(CancellationToken cancellationToken)
        {
            return (CompilationUnitSyntax)await Context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}