using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    public class MemberAccessCodeFixHelper : CodeFixHelper
    {
        public MemberAccessCodeFixHelper(CodeFixContext context): base(context)
        {
        }

        /// <summary>
        /// Returns first <see cref="MemberAccessExpressionSyntax"/> which spans same location as diagnostic.
        /// Any follow up member accesses occurring after diagnostic's span end will not be included.
        /// </summary>
        /// <returns>First member access sharing span with diagnostic; <c>null</c> if no such exists</returns>
        public async Task<MemberAccessExpressionSyntax> GetDiagnosedMemberAccess()
        {
            var root = await GetDocumentRoot();
            var diagnostic = Context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var memberAccessExpressions =
                root.FindNode(diagnosticSpan, true).FirstAncestorOrSelf<MemberAccessExpressionSyntax>();

            return memberAccessExpressions;
        }

        /// <summary>
        /// Returns first <see cref="MemberAccessExpressionSyntax"/> starting at same position as the diagnostic starts
        /// </summary>
        /// <returns>First member access that starts at diagnostic's span start; <c>null</c> if no such exists</returns>
        public async Task<MemberAccessExpressionSyntax> GetClosestMemberAccess()
        {
            var allMemberAccessExpressions = await GetDiagnosedMemberAccesses();

            return allMemberAccessExpressions.FirstOrDefault();
        }

        /// <summary>
        /// Returns outer  most <see cref="MemberAccessExpressionSyntax"/> that includes diagnostic's start position
        /// </summary>
        /// <returns>Outer most member access which has diagnostic start position within; <c>null</c> if no such exists</returns>
        public async Task<MemberAccessExpressionSyntax> GetOuterMostMemberAccess()
        {
            var allMemberAccessExpressions = await GetDiagnosedMemberAccesses();

            return allMemberAccessExpressions.LastOrDefault();
        }

        /// <summary>
        /// Returns all <see cref="MemberAccessExpressionSyntax"/> that  includes diagnostic's start position
        /// </summary>
        /// <returns>All member accesses containing diagnostic's start position</returns>
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
