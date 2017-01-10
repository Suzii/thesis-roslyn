using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers.CodeFixes
{
    public class MemberInvocationCodeFixHelper : CodeFixHelper
    {
        public MemberInvocationCodeFixHelper(CodeFixContext context): base(context)
        {
        }

        /// <summary>
        /// Returns first <see cref="InvocationExpressionSyntax"/> which spans same location as diagnostic.
        /// Any follow up invocation expression occurring after diagnostic's span end will not be included.
        /// </summary>
        /// <returns>First invocation sharing span with diagnostic; <c>null</c> if no such exists</returns>
        public async Task<InvocationExpressionSyntax> GetDiagnosedInvocation()
        {
            var root = await GetDocumentRoot();
            var diagnostic = Context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var invocationExpression =
                root.FindNode(diagnosticSpan, true).FirstAncestorOrSelf<InvocationExpressionSyntax>();

            return invocationExpression;
        }

        ///// <summary>
        ///// Returns first <see cref="InvocationExpressionSyntax"/> starting at same position as the diagnostic starts
        ///// </summary>
        ///// <returns>First invocation that starts at diagnostic's span start; <c>null</c> if no such exists</returns>
        //public async Task<InvocationExpressionSyntax> GetClosestInvocation()
        //{
        //    var allInvocationExpressions = await GetDiagnosedInvocations();

        //    return allInvocationExpressions.FirstOrDefault();
        //}

        ///// <summary>
        ///// Returns outer  most <see cref="InvocationExpressionSyntax"/> that includes diagnostic's start position
        ///// </summary>
        ///// <returns>Outer most invocation which has diagnostic start position within; <c>null</c> if no such exists</returns>
        //public async Task<InvocationExpressionSyntax> GetOuterMostInvocation()
        //{
        //    var allInvocationExpressions = await GetDiagnosedInvocations();

        //    return allInvocationExpressions.LastOrDefault();
        //}

        /// <summary>
        /// Returns all <see cref="InvocationExpressionSyntax"/> that  includes diagnostic's start position
        /// </summary>
        /// <returns>All invocations containing diagnostic's start position</returns>
        private async Task<IEnumerable<InvocationExpressionSyntax>> GetDiagnosedInvocations()
        {
            var root = await GetDocumentRoot();
            var diagnostic = Context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var invocationExpressions =
                root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<InvocationExpressionSyntax>();

            return invocationExpressions;
        }
    }
}
