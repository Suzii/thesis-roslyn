using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BugHunter.Core.Extensions
{
    public static class InvocationExpressionSyntaxExtensions
    {
        /// <summary>
        /// Tries to extract node representing name of the invoked method.
        /// Returns true if the method name node could be found and populates <param name="methodNameNode"></param> with it
        /// </summary>
        /// <param name="invocationExpression">Invocation invocationExpression</param>
        /// <param name="methodNameNode">Out parameter to be populated with found method name node</param>
        /// <returns>True if name of the invoked method was found</returns>
        public static bool TryGetMethodNameNode(this InvocationExpressionSyntax invocationExpression, out SimpleNameSyntax methodNameNode)
        {
            var expressionKind = invocationExpression?.Expression?.Kind();
            switch (expressionKind)
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                    var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;
                    methodNameNode = memberAccess.Name;
                    break;
                case SyntaxKind.MemberBindingExpression:
                    var memberBinding = (MemberBindingExpressionSyntax)invocationExpression.Expression;
                    methodNameNode = memberBinding.Name;
                    break;
                case SyntaxKind.IdentifierName:
                    methodNameNode = (IdentifierNameSyntax)invocationExpression.Expression;
                    break;
                default:
                    methodNameNode = null;
                    break;
            }

            return methodNameNode != null;
        }

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
            
            return invocation.AppendArguments(argumentsToBeAdded.ToArray());
        }
    }
}