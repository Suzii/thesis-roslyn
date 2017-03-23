using System;
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
            var memberAccessExpressionSyntax = invocationExpression?.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null)
            {
                throw new ArgumentException(nameof(invocationExpression));
            }

            return memberAccessExpressionSyntax;
        }
    }
}