using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Diagnostic formatter for <see cref="InvocationExpressionSyntax"/> nodes, where only method name part should be reflected in raised diagnostic
    /// </summary>
    public class MethodInvocationOnlyDiagnosticFormatter : MethodInvocationDiagnosticFormatter
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <param name="descriptor"></param> based on passed <param name="invocationExpression"></param>.
        ///
        /// MessageFormat will be passed a string representation of invoked method name along with argument list of <param name="invocationExpression"></param>.
        /// Location will be only of method name + argument list part of passed <param name="invocationExpression"></param>.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="invocationExpression">Invocation expression that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given invocation expression</returns>
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            var diagnostic = !invocationExpression.TryGetMethodNameNode(out methodNameNode)
                ? Diagnostic.Create(descriptor, Location.None)
                : Diagnostic.Create(descriptor, GetLocation(invocationExpression, methodNameNode), GetDiagnosedUsage(invocationExpression, methodNameNode));

            return MarkDiagnosticIfNecessary(diagnostic, invocationExpression);
        }

        /// <summary>
        /// Returns location of only method name syntaxNode + argument list of invocation.
        ///
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed,
        /// location of 'WhereLike("col", "val")' is returned.
        /// </summary>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>Location of nested method name and arguments</returns>
        protected override Location GetLocation(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return Location.None;
            }

            return GetLocation(invocationExpression, methodNameNode);
        }

        /// <summary>
        /// Returns string representation of only method name syntaxNode + argument list of invocation, without possible whitespaces.
        ///
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed, only 'WhereLike("col", "val")' string is returned.
        /// </summary>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>String representation of nested method name and arguments</returns>
        protected override string GetDiagnosedUsage(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return invocationExpression.ToString();
            }

            return GetDiagnosedUsage(invocationExpression, methodNameNode);
        }

        private Location GetLocation(InvocationExpressionSyntax invocationExpression, SimpleNameSyntax methodNameNode)
        {
            var statLocation = methodNameNode.GetLocation().SourceSpan.Start;
            var endLocation = invocationExpression.GetLocation().SourceSpan.End;
            var location = Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(statLocation, endLocation));

            return location;
        }

        private string GetDiagnosedUsage(InvocationExpressionSyntax invocationExpression, SimpleNameSyntax methodNameNode)
            => $"{methodNameNode.Identifier.ValueText}{invocationExpression.ArgumentList}";
    }
}