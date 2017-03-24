using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class ConditionalAccessDiagnosticFormatter : IDiagnosticFormatter<ConditionalAccessExpressionSyntax>
    {
        public Location GetLocation(ConditionalAccessExpressionSyntax expression)
        {
            var firstMemberBindingExpressionOnTheRightOfTheDot = expression?.GetFirstMemberBindingExpression();
            if (firstMemberBindingExpressionOnTheRightOfTheDot == null)
            {
                return Location.None;
            }

            var sourceSpanEnd = firstMemberBindingExpressionOnTheRightOfTheDot.GetLocation().SourceSpan.End;
            var sourceSpanStart = expression.GetLocation().SourceSpan.Start;
            var location = Location.Create(expression.SyntaxTree, TextSpan.FromBounds(sourceSpanStart, sourceSpanEnd));

            return location;
        }

        public string GetDiagnosedUsage(ConditionalAccessExpressionSyntax expression)
        {
            var firstMemberBindingExpressionOnTheRightOfTheDot = expression?.GetFirstMemberBindingExpression();
            if (firstMemberBindingExpressionOnTheRightOfTheDot == null)
            {
                return string.Empty;
            }

            return $"{expression.Expression}{expression.OperatorToken}{firstMemberBindingExpressionOnTheRightOfTheDot.OperatorToken}{firstMemberBindingExpressionOnTheRightOfTheDot.Name}";
        }
    }
}