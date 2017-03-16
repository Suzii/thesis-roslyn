using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            return await ApplyRootModification((oldRoot) => oldRoot.ReplaceNode(oldNode, newNode.WithTriviaFrom(oldNode)), namespacesToBeReferenced);
        }

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

        public Diagnostic GetFirstDiagnostic()
        {
            return Context.Diagnostics.First();
        }

        public Diagnostic GetFirstDiagnostic(params string[] diagnosticIds)
        {
            return Context.Diagnostics.FirstOrDefault(diagnostic => diagnosticIds.Contains(diagnostic.Id));
        }

        public Location GetDiagnosticLocation()
        {
            return GetFirstDiagnostic().Location;
        }

        protected async Task<CompilationUnitSyntax> GetDocumentRoot()
        {
            return (CompilationUnitSyntax) await Context.Document.GetSyntaxRootAsync(Context.CancellationToken).ConfigureAwait(false);
        }
    }
}