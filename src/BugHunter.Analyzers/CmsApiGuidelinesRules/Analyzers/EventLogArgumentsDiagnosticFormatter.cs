using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    internal class EventLogArgumentsDiagnosticFormatter : IDiagnosticFormatter<InvocationExpressionSyntax>
    {
        public Location GetLocation(InvocationExpressionSyntax expression)
        {
            var invocationExpression = expression as InvocationExpressionSyntax;
            return invocationExpression?.ArgumentList.Arguments.First().GetLocation();
        }

        public string GetDiagnosedUsage(InvocationExpressionSyntax expression)
        {
            var invocationExpression = expression as InvocationExpressionSyntax;
            return invocationExpression?.ArgumentList.Arguments.First().ToString();
        }
    }
}