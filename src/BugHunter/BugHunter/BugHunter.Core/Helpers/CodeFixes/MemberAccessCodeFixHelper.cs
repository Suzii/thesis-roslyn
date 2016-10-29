using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Helpers.CodeFixes
{
    public class MemberAccessCodeFixHelper : CodeFixHelper
    {
        public MemberAccessCodeFixHelper(CodeFixContext context): base(context)
        {
        }

        public async Task<MemberAccessExpressionSyntax> GetClosestMemberAccess()
        {
            var allMemberAccessExpressions = await GetDiagnosedMemberAccesses();

            return allMemberAccessExpressions.First();
        }

        public async Task<MemberAccessExpressionSyntax> GetOutterMostMemberAccess()
        {
            var allMemberAccessExpressions = await GetDiagnosedMemberAccesses();

            return allMemberAccessExpressions.Last();
        }

        private async Task<IEnumerable<MemberAccessExpressionSyntax>> GetDiagnosedMemberAccesses()
        {
            var root = await GetDocumentRoot();
            var diagnostic = Context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var memberAccessExpressions =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MemberAccessExpressionSyntax>();

            return memberAccessExpressions;
        }
    }
}
