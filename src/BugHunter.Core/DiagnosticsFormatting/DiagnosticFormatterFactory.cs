using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting
{
    public static class DiagnosticFormatterFactory
    {
        public static ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> CreateMemberAccessFormatter()
            =>new MemberAccessDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> CreateMemberInvocationFormatter()
            => new MethodInvocationDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> CreateMemberInvocationOnlyFormatter(bool stripOfArgsFromMessage = false)
            => stripOfArgsFromMessage
            ? (ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax>) new MethodInvocationOnlyNoArgsDiagnosticFormatter()        
            : (ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax>) new MethodInvocationOnlyDiagnosticFormatter();

        public static ISyntaxNodeDiagnosticFormatter<ConditionalAccessExpressionSyntax> CreateConditionalAccessFormatter()
            => new ConditionalAccessDiagnosticFormatter();

        public static ISymbolDiagnosticFormatter<INamedTypeSymbol> CreateNamedTypeSymbolFormatter()
            => new NamedTypeSymbolDiagnosticFormatter();
    }
}