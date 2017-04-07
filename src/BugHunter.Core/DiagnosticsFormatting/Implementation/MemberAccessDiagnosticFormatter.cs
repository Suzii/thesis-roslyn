using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessDiagnosticFormatter : DefaultDiagnosticFormatter<MemberAccessExpressionSyntax>
    {
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, MemberAccessExpressionSyntax syntaxNode)
            => base.CreateDiagnostic(descriptor, syntaxNode).MarkAsSimpleMemberAccess();

        public override string GetDiagnosedUsage(MemberAccessExpressionSyntax syntaxNode)
            => syntaxNode == null ? string.Empty : $"{syntaxNode.Expression}.{syntaxNode.Name}";
    }
}