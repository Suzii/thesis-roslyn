using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberInvocationDiagnosticFormatterBase
    {
        /// <summary>
        /// Returns <see cref="MemberAccessExpressionSyntax"/> that is wrapped by <param name="invocationExpression"></param>
        /// 
        /// Throws <see cref="ArgumentException"/> if unable to cast <param name="invocationExpression"></param> to <see cref="MemberAccessExpressionSyntax"/>
        /// </summary>
        /// <param name="invocationExpression">Invocation invocationExpression</param>
        /// <returns>Underlying member access</returns>
        protected static MemberAccessExpressionSyntax GetUnderlyingMemberAccess(InvocationExpressionSyntax invocationExpression)
        {
            var invocationExpressionSyntax = invocationExpression as InvocationExpressionSyntax;
            var memberAccessExpressionSyntax = invocationExpressionSyntax?.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null)
            {
                throw new ArgumentException(nameof(invocationExpression));
            }

            return memberAccessExpressionSyntax;
        }

        /// <summary>
        /// Returns <see cref="InvocationExpressionSyntax"/> casted from <param name="expression"></param>
        /// 
        /// Throws <see cref="ArgumentException"/> if unable to cast <param name="expression"></param> to <see cref="InvocationExpressionSyntax"/>
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Casted invocation expression</returns>
        protected static InvocationExpressionSyntax GetInvocationExpression(SyntaxNode expression)
        {
            var invocationExpressionSyntax = expression as InvocationExpressionSyntax;
            if (invocationExpressionSyntax == null)
            {
                throw new ArgumentException(nameof(expression));
            }

            return invocationExpressionSyntax;
        }
    }
}