using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class ConditionalAccessDiagnosticFormatter : DefaultDiagnosticFormatter<ConditionalAccessExpressionSyntax>
    {
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, ConditionalAccessExpressionSyntax syntaxNode)
        {
            var firstMemberBinding = syntaxNode?.GetFirstMemberBindingExpression();
            if (firstMemberBinding == null)
            {
                return Diagnostic.Create(descriptor, Location.None);
            }

            var location = GetLocation(syntaxNode, firstMemberBinding);
            var diagnosedUsage = GetDiagnosedUsage(syntaxNode, firstMemberBinding);

            return Diagnostic.Create(descriptor, location, diagnosedUsage).MarkAsConditionalAccess();
        }

        public override Location GetLocation(ConditionalAccessExpressionSyntax syntaxNode)
        {
            var firstMemberBindingExpressionOnTheRightOfTheDot = syntaxNode?.GetFirstMemberBindingExpression();
            if (firstMemberBindingExpressionOnTheRightOfTheDot == null)
            {
                return Location.None;
            }
            
            var location = GetLocation(syntaxNode, firstMemberBindingExpressionOnTheRightOfTheDot);

            return location;
        }

        public override string GetDiagnosedUsage(ConditionalAccessExpressionSyntax syntaxNode)
        {
            var firstMemberBinding = syntaxNode?.GetFirstMemberBindingExpression();

            return firstMemberBinding == null 
                ? string.Empty 
                : GetDiagnosedUsage(syntaxNode, firstMemberBinding);
        }

        private Location GetLocation(ConditionalAccessExpressionSyntax conditionalAccessExpression, MemberBindingExpressionSyntax firstMemberBindingExpression)
        {
            var sourceSpanEnd = firstMemberBindingExpression.GetLocation().SourceSpan.End;
            var sourceSpanStart = conditionalAccessExpression.GetLocation().SourceSpan.Start;
            var location = Location.Create(conditionalAccessExpression.SyntaxTree, TextSpan.FromBounds(sourceSpanStart, sourceSpanEnd));

            return location;
        }

        private string GetDiagnosedUsage(ConditionalAccessExpressionSyntax syntaxNode, MemberBindingExpressionSyntax firstMemberBinding)
            => $"{syntaxNode.Expression}{syntaxNode.OperatorToken}{firstMemberBinding.OperatorToken}{firstMemberBinding.Name}";
    }
}