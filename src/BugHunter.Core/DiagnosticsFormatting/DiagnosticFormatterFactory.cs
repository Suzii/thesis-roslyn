using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public static class DiagnosticFormatterFactory
    {
        public static ISyntaxNodeDiagnosticFormatter<SyntaxNode> CreateDefaultFormatter()
            => new DefaultDiagnosticFormatter<SyntaxNode>();

        public static ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> CreateMemberAccessFormatter()
            =>new MemberAccessDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> CreateMemberAccessOnlyFormatter()
            => new MemberAccessOnlyDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> CreateMemberInvocationFormatter()
            => new MemberInvocationDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> CreateMemberInvocationOnlyFormatter(bool stripOfArgsFromMessage = false)
            => stripOfArgsFromMessage
            ? (ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax>) new MemberInvocationOnlyNoArgsDiagnosticFormatter()        
            : (ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax>) new MemberInvocationOnlyDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<ConditionalAccessExpressionSyntax> CreateConditionalAccessFormatter()
            => new ConditionalAccessDiagnosticFormatter();


        public static ISyntaxNodeDiagnosticFormatter<TSyntaxNode> CreateFormatter<TFormatter, TSyntaxNode>()
            where TFormatter : ISyntaxNodeDiagnosticFormatter<TSyntaxNode>, new()
            where TSyntaxNode : SyntaxNode
        {
            return new TFormatter();
        }
    }
}