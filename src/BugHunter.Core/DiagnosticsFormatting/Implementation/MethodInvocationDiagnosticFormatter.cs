using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Default diagnostic formatter for <see cref="InvocationExpressionSyntax"/> nodes
    /// </summary>
    public class MethodInvocationDiagnosticFormatter : DefaultDiagnosticFormatter<InvocationExpressionSyntax>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <paramref name="descriptor" /> based on passed <paramref name="invocationExpression" />.
        ///
        /// MessageFormat will be passed a string representation of <paramref name="invocationExpression" />.
        /// Location will be of a whole <paramref name="invocationExpression" />.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="invocationExpression">Invocation that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given invocation</returns>
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, InvocationExpressionSyntax invocationExpression)
        {
            var diagnostic = base.CreateDiagnostic(descriptor, invocationExpression);
            var markedDiagnostic = MarkDiagnosticIfNecessary(diagnostic, invocationExpression);

            return markedDiagnostic;
        }

        /// <summary>
        /// Diagnostic will be marked as being raised on conditional or simple member access, based on the underlying kind of 'Expression' property
        /// </summary>
        /// <param name="diagnostic">Diagnostic to be marked</param>
        /// <param name="invocationExpression">Invocation that the diagnostic is being raised for</param>
        /// <returns>Potentially marked diagnostic</returns>
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