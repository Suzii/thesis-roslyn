using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessOnlyDiagnosticFormatter : DefaultDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        public override Location GetLocation(MemberAccessExpressionSyntax syntaxNode)
            => syntaxNode?.Name?.GetLocation();

        public override string GetDiagnosedUsage(MemberAccessExpressionSyntax syntaxNode)
            => syntaxNode?.Name?.Identifier.ValueText;
    }
}