using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    /// <summary>
    /// Class handling common tasks related to code fixes of <see cref="InvocationExpressionSyntax"/>. Always instantiated with concrete <see cref="CodeFixContext"/> instance.
    /// </summary>
    public class MemberInvocationCodeFixHelper : CodeFixHelper
    {
        public MemberInvocationCodeFixHelper(CodeFixContext context): base(context)
        {
        }

        /// <summary>
        /// Returns first <see cref="InvocationExpressionSyntax"/> which spans same location as diagnostic.
        /// Any follow up invocation expression occurring after diagnostic's span end will not be included.
        /// </summary>
        /// <returns>First invocation sharing same span with diagnostic; <c>null</c> if no such exists</returns>
        public async Task<InvocationExpressionSyntax> GetDiagnosedInvocation()
        {
            var root = await GetDocumentRoot();
            var diagnosticSpan = GetDiagnosticLocation().SourceSpan;

            var invocationExpression = root.FindNode(diagnosticSpan, true).AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault();

            return invocationExpression;
        }
    }
}
