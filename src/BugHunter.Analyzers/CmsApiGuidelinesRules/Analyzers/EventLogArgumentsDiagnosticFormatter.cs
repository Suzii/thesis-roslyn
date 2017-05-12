using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    /// <summary>
    /// Diagnostic formatter for <see cref="InvocationExpressionSyntax"/> nodes, where only first argument passed should be reflected in the diagnostic
    /// </summary>
    internal class EventLogArgumentsDiagnosticFormatter : DefaultDiagnosticFormatter<InvocationExpressionSyntax>
    {
        /// <summary>
        /// Returns location of only the first argument of the invocation. 
        /// </summary>
        /// <remarks>
        /// E.g. if Invocation like 'Method("col", "val")' is passed, location of "col" is returned.
        /// </remarks>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>Location of the first argument of the invocation</returns>
        protected override Location GetLocation(InvocationExpressionSyntax invocationExpression)
            => invocationExpression?.ArgumentList.Arguments.First().GetLocation();

        /// <summary>
        /// Returns string representation of only the first argument of the invocation. 
        /// </summary>
        /// <remarks>
        /// E.g. if Invocation like 'Method("col", "val")' is passed, "col" is returned.
        /// </remarks>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>String representation of the first argument of the invocation</returns>
        protected override string GetDiagnosedUsage(InvocationExpressionSyntax invocationExpression)
            => invocationExpression?.ArgumentList.Arguments.First().ToString();
    }
}