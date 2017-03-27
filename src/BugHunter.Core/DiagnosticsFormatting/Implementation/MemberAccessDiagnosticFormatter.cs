using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessDiagnosticFormatter : DefaultDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        public override string GetDiagnosedUsage(MemberAccessExpressionSyntax syntaxNode)
            => syntaxNode == null ? string.Empty : $"{syntaxNode.Expression}.{syntaxNode.Name}";
    }
}