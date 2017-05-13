using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    /// <summary>
    /// Class handling common tasks related to code fixes of <see cref="MemberAccessExpressionSyntax"/>. Always instantiated with concrete <see cref="CodeFixContext"/> instance.
    /// </summary>
    public class MemberAccessCodeFixHelper : CodeFixHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAccessCodeFixHelper"/> class.
        /// </summary>
        /// <param name="context">Context of the code fix</param>
        public MemberAccessCodeFixHelper(CodeFixContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Returns first <see cref="MemberAccessExpressionSyntax"/> which spans same location as first diagnostic.
        /// Any follow up member accesses occurring after diagnostic's span end will not be included.
        /// </summary>
        /// <returns>First member access sharing span with diagnostic; <c>null</c> if no such exists</returns>
        public async Task<MemberAccessExpressionSyntax> GetDiagnosedMemberAccess()
        {
            var root = await GetDocumentRoot();
            var diagnosticSpan = GetDiagnosticLocation().SourceSpan;

            var memberAccessExpressions = root.FindNode(diagnosticSpan, true) as MemberAccessExpressionSyntax;

            return memberAccessExpressions;
        }
    }
}
