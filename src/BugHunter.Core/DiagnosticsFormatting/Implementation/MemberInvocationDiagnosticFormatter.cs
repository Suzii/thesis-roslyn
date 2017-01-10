using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberInvocationDiagnosticFormatter : MemberInvocationDiagnosticFormatterBase, IDiagnosticFormatter
    {
        public Location GetLocation(SyntaxNode expression)
        {
            return expression.GetLocation();
        }

        public string GetDiagnosedUsage(SyntaxNode expression)
        {
            return $"{expression}";
        }
    }
}