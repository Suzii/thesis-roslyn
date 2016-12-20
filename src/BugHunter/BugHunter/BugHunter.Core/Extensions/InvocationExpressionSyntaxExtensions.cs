using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace BugHunter.Core.Extensions
{
    public static class InvocationExpressionSyntaxExtensions
    {
        /// <summary>
        /// Appends <param name="newArguments"></param> to <param name="invocation"></param>
        /// </summary>
        /// <param name="invocation">Invocation expression</param>
        /// <param name="newArguments">Arguments to be appended</param>
        /// <returns>Invocation expression with newly added arguments, or old invocation if no arguments were passed</returns>
        public static InvocationExpressionSyntax AppendArguments(this InvocationExpressionSyntax invocation, params ArgumentSyntax[] newArguments)
        {
            if (newArguments == null || newArguments.Length == 0)
            {
                return invocation;
            }

            var newMethodArguments = invocation.ArgumentList.Arguments.AddRange(newArguments);
            var newArgumentList = SyntaxFactory.ArgumentList(newMethodArguments);
            var newInvocation = invocation.WithArgumentList(newArgumentList).NormalizeWhitespace();

            return newInvocation;
        }

        /// <summary>
        /// Appends <param name="newArguments"></param> to <param name="invocation"></param>
        /// </summary>
        /// <param name="invocation">Invocation expression</param>
        /// <param name="newArguments">Arguments to be appended</param>
        /// <returns>Invocation expression with newly added arguments, or old invocation if no arguments were passed</returns>
        public static InvocationExpressionSyntax AppendArguments(this InvocationExpressionSyntax invocation, params string[] newArguments)
        {
            if (newArguments == null || newArguments.Length == 0)
            {
                return invocation;
            }

            var argumentsToBeAdded = newArguments.Select(a => SyntaxFactory.Argument(SyntaxFactory.ParseExpression(a)));
            var newMethodArguments = invocation.ArgumentList.Arguments.AddRange(argumentsToBeAdded);
            var newArgumentList = SyntaxFactory.ArgumentList(newMethodArguments);
            var newInvocation = invocation.WithArgumentList(newArgumentList).NormalizeWhitespace();

            return newInvocation;
        }
    }
}