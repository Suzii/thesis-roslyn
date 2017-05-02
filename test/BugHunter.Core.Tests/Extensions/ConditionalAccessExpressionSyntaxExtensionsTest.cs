using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace BugHunter.Core.Tests.Extensions
{
    [TestFixture]
    public class ConditionalAccessExpressionSyntaxExtensionsTest
    {
        class GetFirstMemberBindingExpression
        {
            [Test]
            public void OnConditionalAccess_ReturnsFirstMemberBinding()
            {
                var expression = SyntaxFactory.ParseName(@"this.is.some.nested.access");
                var memberBindingExpression = SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(@"AndThisIsConditionallyAccessed"));

                var conditionalAccessExpression = SyntaxFactory.ConditionalAccessExpression(expression, memberBindingExpression);
                var expected = conditionalAccessExpression.WhenNotNull;
                var actual = conditionalAccessExpression.GetFirstMemberBindingExpression();

                Assert.AreEqual(expected, actual);
            }
        }
    }
}