using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberInvocationOnlyNoArgsDiagnosticFormatter : IDiagnosticFormatter<InvocationExpressionSyntax>
    {
        /// <summary>
        /// Returns location of method invocation only. 
        /// 
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed,
        /// location of 'WhereLike("col", "val")' is returned. And message will contain WhereLike() only
        /// Throws if <param name="invocationExpression"></param> cannot be casted to <see cref="MemberAccessExpressionSyntax"/>
        /// </summary>
        /// <param name="invocationExpression">Invocation expression</param>
        /// <returns>Location of nested method invocation</returns>
        public Location GetLocation(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return Location.None;
            }

            var statLocation = methodNameNode.GetLocation().SourceSpan.Start;
            var endLocation = invocationExpression.GetLocation().SourceSpan.End;
            var location = Location.Create(invocationExpression.SyntaxTree, TextSpan.FromBounds(statLocation, endLocation));

            return location;
        }

        public string GetDiagnosedUsage(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return $"{invocationExpression.Expression}()";
            }

            return $"{methodNameNode.Identifier.ValueText}()";
        }
    }
}