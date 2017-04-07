using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace BugHunter.Core.DiagnosticsFormatting.Implementation
{
    internal class MemberInvocationOnlyDiagnosticFormatter : MemberInvocationDiagnosticFormatter
    {
        public override Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, InvocationExpressionSyntax syntaxNode)
        {
            SimpleNameSyntax methodNameNode;
            Diagnostic diagnostic;
            if (!syntaxNode.TryGetMethodNameNode(out methodNameNode))
            {
                diagnostic = Diagnostic.Create(descriptor, Location.None);
            }
            else
            {
                diagnostic = Diagnostic.Create(descriptor, GetLocation(syntaxNode, methodNameNode), GetDiagnosedUsage(syntaxNode, methodNameNode));
            }

            return MarkDiagnosticIfNecessary(diagnostic, syntaxNode);
        }

        /// <summary>
        /// Returns location of method syntaxNode only. 
        /// 
        /// E.g. if Invocation like 'condition.Or().WhereLike("col", "val")' is passed,
        /// location of 'WhereLike("col", "val")' is returned.
        /// Throws if <param name="syntaxNode"></param> cannot be casted to <see cref="MemberAccessExpressionSyntax"/>
        /// </summary>
        /// <param name="syntaxNode">Invocation syntaxNode</param>
        /// <returns>Location of nested method syntaxNode</returns>
        public override Location GetLocation(InvocationExpressionSyntax syntaxNode)
        {
            SimpleNameSyntax methodNameNode;
            if (!syntaxNode.TryGetMethodNameNode(out methodNameNode))
            {
                return Location.None;
            }

            return GetLocation(syntaxNode, methodNameNode);
        }

        public override string GetDiagnosedUsage(InvocationExpressionSyntax syntaxNode)
        {
            SimpleNameSyntax methodNameNode;
            if (!syntaxNode.TryGetMethodNameNode(out methodNameNode))
            {
                return syntaxNode.ToString();
            }

            return GetDiagnosedUsage(syntaxNode, methodNameNode);
        }

        private Location GetLocation(InvocationExpressionSyntax syntaxNode, SimpleNameSyntax methodNameNode)
        {
            var statLocation = methodNameNode.GetLocation().SourceSpan.Start;
            var endLocation = syntaxNode.GetLocation().SourceSpan.End;
            var location = Location.Create(syntaxNode.SyntaxTree, TextSpan.FromBounds(statLocation, endLocation));

            return location;
        }

        private string GetDiagnosedUsage(InvocationExpressionSyntax syntaxNode, SimpleNameSyntax methodNameNode)
            => $"{methodNameNode.Identifier.ValueText}{syntaxNode.ArgumentList}";
    }
}