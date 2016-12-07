using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Helpers
{
    public static class MethodInvocationHelper
    {
        /// <summary>
        /// Returns <see cref="MemberAccessExpressionSyntax"/> that is wrapped by <param name="invocationExpression"></param>
        /// 
        /// Throws <see cref="ArgumentException"/> if unable to cast <param name="invocationExpression"></param> to <see cref="MemberAccessExpressionSyntax"/>
        /// </summary>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>Underlying member access</returns>
        public static MemberAccessExpressionSyntax GetUnderlyingMemberAccess(InvocationExpressionSyntax invocationExpression)
        {
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            if (memberAccess == null)
            {
                throw new ArgumentException(@"Unable to cast to MemberAccessExpression", nameof(invocationExpression));
            }

            return memberAccess;
        }


        public static string GetMethodName(InvocationExpressionSyntax invocationExpression)
        {
            var memberAccess = GetUnderlyingMemberAccess(invocationExpression);

            return memberAccess.Name.ToString();
        }
    }
}