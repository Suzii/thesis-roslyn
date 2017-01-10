using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace BugHunter.Core.Tests.Extensions
{
    [TestFixture]
    public class InvocationExpressionSyntaxExtenstions
    {
        [TestCase(@"someMethod()", @"someMethod(""newArgument"")")]
        [TestCase(@"someMethod(""a"", ""b"")", @"someMethod(""a"", ""b"", ""newArgument"")")]
        public void OneStringArgument_ReturnsUpdatedInvocation(string invocation, string updatedInvocation)
        {
            var original = SyntaxFactory.ParseExpression(invocation) as InvocationExpressionSyntax;
            var expected = SyntaxFactory.ParseExpression(updatedInvocation) as InvocationExpressionSyntax;
            var updated = original.AppendArguments(@"""newArgument""");

            Assert.AreEqual(expected.ToString(), updated.ToString());
        }

        [TestCase(@"someMethod()", @"someMethod(""newArgument1"", ""newArgument2"")")]
        [TestCase(@"someMethod(""a"", ""b"")", @"someMethod(""a"", ""b"", ""newArgument1"", ""newArgument2"")")]
        public void MoreStringArguments_ReturnsUpdatedInvocation(string invocation, string updatedInvocation)
        {
            var original = SyntaxFactory.ParseExpression(invocation) as InvocationExpressionSyntax;
            var expected = SyntaxFactory.ParseExpression(updatedInvocation) as InvocationExpressionSyntax;
            var updated = original.AppendArguments(@"""newArgument1""", @"""newArgument2""");

            Assert.AreEqual(expected.ToString(), updated.ToString());
        }

        [TestCase(@"someMethod()", @"someMethod(""newArgument"")")]
        [TestCase(@"someMethod(""a"", ""b"")", @"someMethod(""a"", ""b"", ""newArgument"")")]
        public void OneArgument_ReturnsUpdatedInvocation(string invocation, string updatedInvocation)
        {
            var original = SyntaxFactory.ParseExpression(invocation) as InvocationExpressionSyntax;
            var expected = SyntaxFactory.ParseExpression(updatedInvocation) as InvocationExpressionSyntax;
            var newArgument = SyntaxFactory.Argument(SyntaxFactory.ParseExpression(@"""newArgument"""));
            var updated = original.AppendArguments(newArgument);

            Assert.AreEqual(expected.ToString(), updated.ToString());
        }

        [TestCase(@"someMethod()", @"someMethod(""newArgument1"", ""newArgument2"")")]
        [TestCase(@"someMethod(""a"", ""b"")", @"someMethod(""a"", ""b"", ""newArgument1"", ""newArgument2"")")]
        public void MoreArguments_ReturnsUpdatedInvocation(string invocation, string updatedInvocation)
        {
            var original = SyntaxFactory.ParseExpression(invocation) as InvocationExpressionSyntax;
            var expected = SyntaxFactory.ParseExpression(updatedInvocation) as InvocationExpressionSyntax;
            var newArgument1 = SyntaxFactory.Argument(SyntaxFactory.ParseExpression(@"""newArgument1"""));
            var newArgument2 = SyntaxFactory.Argument(SyntaxFactory.ParseExpression(@"""newArgument2"""));

            var updated = original.AppendArguments(newArgument1, newArgument2);

            Assert.AreEqual(expected.ToString(), updated.ToString());
        }
    }
}