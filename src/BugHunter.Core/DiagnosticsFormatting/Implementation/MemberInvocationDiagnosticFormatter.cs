using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberInvocationDiagnosticFormatter : DefaultDiagnosticFormatter<InvocationExpressionSyntax>
    {
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, InvocationExpressionSyntax invocationExpression)
        {
            var diagnostic = base.CreateDiagnostic(descriptor, invocationExpression);
            var markedDiagnostic = MarkDiagnosticIfNecessary(diagnostic, invocationExpression);

            return markedDiagnostic;
        }

        protected Diagnostic MarkDiagnosticIfNecessary(Diagnostic diagnostic, InvocationExpressionSyntax invocationExpression)
        {
            var expression = invocationExpression?.Expression;
            var expressionKind = expression?.Kind();

            switch (expressionKind)
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    return diagnostic.MarkAsSimpleMemberAccess();
                case SyntaxKind.MemberBindingExpression:
                    return diagnostic.MarkAsConditionalAccess();
                default:
                    return diagnostic;
            }
        }
    }
}