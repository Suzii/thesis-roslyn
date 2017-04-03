using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers
{
    internal class EventLogArgumentsDiagnosticFormatter : DefaultDiagnosticFormatter<InvocationExpressionSyntax>
    {
        public override Location GetLocation(InvocationExpressionSyntax invocationExpression)
            => invocationExpression?.ArgumentList.Arguments.First().GetLocation();

        public override string GetDiagnosedUsage(InvocationExpressionSyntax invocationExpression)
            => invocationExpression?.ArgumentList.Arguments.First().ToString();
    }
}