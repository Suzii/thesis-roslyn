using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Helpers.CodeFixes
{
    internal class MemberAccessCodeFixHelper
    {
        private readonly CodeFixContext _context;
        
        private SyntaxNode _documentRootCache;
        
        public MemberAccessCodeFixHelper(CodeFixContext context)
        {
            _context = context;
        }

        public async Task<MemberAccessExpressionSyntax> GetClosestMemberAccess()
        {
            var root = await GetDocumentRoot();
            var diagnostic = _context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var memberAccessExpression = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>().First();
            
            return memberAccessExpression;
        }

        public async Task<Document> ReplaceWithHelper(MemberAccessExpressionSyntax oldMemberAccess, ExpressionSyntax newMemberAccess, string namepsaceToBeReferenced)
        {
            var document = _context.Document;
            var root = await GetDocumentRoot();

            var formattedMemberAccess = newMemberAccess.WithTriviaFrom(oldMemberAccess);

            var newRoot = root.ReplaceNode(oldMemberAccess, formattedMemberAccess);
            newRoot = UsingsHelper.EnsureUsing((CompilationUnitSyntax)newRoot, namepsaceToBeReferenced);

            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private async Task<SyntaxNode> GetDocumentRoot()
        {
            if (_documentRootCache == null)
            {
                _documentRootCache = await _context.Document.GetSyntaxRootAsync(_context.CancellationToken).ConfigureAwait(false);
            }

            return _documentRootCache;
        }
    }
}
