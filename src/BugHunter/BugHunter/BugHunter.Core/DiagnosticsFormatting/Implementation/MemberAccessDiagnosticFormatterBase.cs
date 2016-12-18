using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberAccessDiagnosticFormatterBase
    {
        protected static MemberAccessExpressionSyntax GetMemberAccess(ExpressionSyntax expression)
        {
            var memberAccessExpressionSyntax = expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null)
            {
                throw new ArgumentException(nameof(expression));
            }

            return memberAccessExpressionSyntax;
        }
    }
}