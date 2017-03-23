using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public static class DiagnosticFormatterFactory
    {
        public static IDiagnosticFormatter<SyntaxNode> CreateDefaultFormatter()
            => new DefaultDiagnosticFormatter();

        public static IDiagnosticFormatter<MemberAccessExpressionSyntax> CreateMemberAccessFormatter()
            =>new MemberAccessDiagnosticFormatter();

        public static IDiagnosticFormatter<MemberAccessExpressionSyntax> CreateMemberAccessOnlyFormatter()
            => new MemberAccessOnlyDiagnosticFormatter();

        public static IDiagnosticFormatter<InvocationExpressionSyntax> CreateMemberInvocationFormatter()
            => new MemberInvocationDiagnosticFormatter();

        public static IDiagnosticFormatter<InvocationExpressionSyntax> CreateMemberInvocationOnlyFormatter(bool stripOfArgsFromMessage = false)
            => stripOfArgsFromMessage
            ? (IDiagnosticFormatter<InvocationExpressionSyntax>) new MemberInvocationOnlyNoArgsDiagnosticFormatter()        
            : (IDiagnosticFormatter<InvocationExpressionSyntax>) new MemberInvocationOnlyDiagnosticFormatter();

        public static IDiagnosticFormatter<ConditionalAccessExpressionSyntax> CreateConditionalAccessFormatter()
            => new ConditionalAccessDiagnosticFormatter();


        public static IDiagnosticFormatter<TSyntaxNode> CreateFormatter<TFormatter, TSyntaxNode>()
            where TFormatter : IDiagnosticFormatter<TSyntaxNode>, new()
            where TSyntaxNode : SyntaxNode
        {
            return new TFormatter();
        }
    }
}