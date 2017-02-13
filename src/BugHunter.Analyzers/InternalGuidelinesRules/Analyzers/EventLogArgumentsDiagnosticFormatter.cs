using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.InternalGuidelinesRules.Analyzers
{
    internal class EventLogArgumentsDiagnosticFormatter : IDiagnosticFormatter
    {
        public Location GetLocation(SyntaxNode expression)
        {
            var invocationExpression = expression as InvocationExpressionSyntax;
            return invocationExpression?.ArgumentList.Arguments.First().GetLocation();
        }

        public string GetDiagnosedUsage(SyntaxNode expression)
        {
            var invocationExpression = expression as InvocationExpressionSyntax;
            return invocationExpression?.ArgumentList.Arguments.First().ToString();
        }
    }
}