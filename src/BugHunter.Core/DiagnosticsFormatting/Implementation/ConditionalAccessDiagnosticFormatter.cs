using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    /// <summary>
    /// Default diagnostic formatter for <see cref="ConditionalAccessExpressionSyntax"/> nodes
    /// </summary>
    internal class ConditionalAccessDiagnosticFormatter : DefaultDiagnosticFormatter<ConditionalAccessExpressionSyntax>
    {
        /// <summary>
        /// Creates a <see cref="Diagnostic"/> from <param name="descriptor"></param> based on passed <param name="conditionalAccess"></param>.
        /// 
        /// MessageFormat will be passed an argument in form 'Expression?.WhenNotNull' of passed <param name="conditionalAccess"></param>.
        /// Location will be of a expression + firs member binding expression of passed<param name="conditionalAccess"></param>.
        /// </summary>
        /// <param name="descriptor">Diagnostic descriptor for diagnostic to be created</param>
        /// <param name="conditionalAccess">Conditional access that the diagnostic should be raised for</param>
        /// <returns>Diagnostic created from descriptor for given conditional access</returns>
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var firstMemberBinding = conditionalAccess?.GetFirstMemberBindingExpression();
            if (firstMemberBinding == null)
            {
                return Diagnostic.Create(descriptor, Location.None);
            }

            var location = GetLocation(conditionalAccess, firstMemberBinding);
            var diagnosedUsage = GetDiagnosedUsage(conditionalAccess, firstMemberBinding);

            return Diagnostic.Create(descriptor, location, diagnosedUsage).MarkAsConditionalAccess();
        }

        /// <summary>
        /// Returns Location of 'expression?.firstMemberBindingName' of passed <param name="conditionalAccess"></param>. 
        /// The follow up part of WhenNotNull property after first member binding expression found will be discarded.
        /// </summary>
        /// <param name="conditionalAccess">Conditional access whose location is being requested</param>
        /// <returns>Location of conditional access up to the end of first member binding expression;
        /// empty location if conditional access is null or member binding was not found</returns>
        protected override Location GetLocation(ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var firstMemberBindingExpressionOnTheRightOfTheDot = conditionalAccess?.GetFirstMemberBindingExpression();
            if (firstMemberBindingExpressionOnTheRightOfTheDot == null)
            {
                return Location.None;
            }
            
            var location = GetLocation(conditionalAccess, firstMemberBindingExpressionOnTheRightOfTheDot);

            return location;
        }

        /// <summary>
        /// Returns string in form 'expression?.firstMemberBindingName' of passed <param name="conditionalAccess"></param>. 
        /// Any whitespaces will be discarded, as will be the rest of the WhenNotNull property after first member binding expression found.
        /// </summary>
        /// <param name="conditionalAccess">Conditional access whose location is being requested</param>
        /// <returns>String representation of conditional access withou whitespaces and follow up part of WhenNotNull property after first member binding expression;
        /// empty string if conditional access is null or member binding was not found</returns>
        protected override string GetDiagnosedUsage(ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var firstMemberBinding = conditionalAccess?.GetFirstMemberBindingExpression();

            return firstMemberBinding == null 
                ? string.Empty 
                : GetDiagnosedUsage(conditionalAccess, firstMemberBinding);
        }

        private Location GetLocation(ConditionalAccessExpressionSyntax conditionalAccessExpression, MemberBindingExpressionSyntax firstMemberBindingExpression)
        {
            var sourceSpanEnd = firstMemberBindingExpression.GetLocation().SourceSpan.End;
            var sourceSpanStart = conditionalAccessExpression.GetLocation().SourceSpan.Start;
            var location = Location.Create(conditionalAccessExpression.SyntaxTree, TextSpan.FromBounds(sourceSpanStart, sourceSpanEnd));

            return location;
        }

        private string GetDiagnosedUsage(ConditionalAccessExpressionSyntax conditionalAccess, MemberBindingExpressionSyntax firstMemberBinding)
            => $"{conditionalAccess.Expression}{conditionalAccess.OperatorToken}{firstMemberBinding.OperatorToken}{firstMemberBinding.Name}";
    }
}