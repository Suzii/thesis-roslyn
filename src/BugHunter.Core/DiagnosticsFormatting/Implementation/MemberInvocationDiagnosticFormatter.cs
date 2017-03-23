using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberInvocationDiagnosticFormatter : MemberInvocationDiagnosticFormatterBase, IDiagnosticFormatter<InvocationExpressionSyntax>
    {
        public Location GetLocation(InvocationExpressionSyntax invocation)
        {
            return invocation?.GetLocation();
        }

        public string GetDiagnosedUsage(InvocationExpressionSyntax invocation)
        {
            return $"{invocation}";
        }
    }
}