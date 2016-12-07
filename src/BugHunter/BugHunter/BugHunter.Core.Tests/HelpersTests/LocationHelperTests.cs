using System;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace BugHunter.Core.Tests.HelpersTests
{
    [TestFixture]
    public class LocationHelperTests
    {
        [TestFixture]
        public class GetLocationOfMethodInvocationOnlyTests
        {
            [Test]
            public void SimpleInvocation()
            {
                var invocation =
                    SyntaxFactory.ParseExpression(@"someClass.MethodA()") as InvocationExpressionSyntax;

                var actualLocation = LocationHelper.GetLocationOfMethodInvocationOnly(invocation);
                var expectedLocation = Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(10, 19));

                Assert.AreEqual(expectedLocation, actualLocation);
            }

            [Test]
            public void MultipleNestedInvocations()
            {
                var invocation =
                    SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().WhereLike(""val"", ""col"")") as InvocationExpressionSyntax;

                var actualLocation = LocationHelper.GetLocationOfMethodInvocationOnly(invocation);
                var expectedLocation = Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(41, 64));

                Assert.AreEqual(expectedLocation, actualLocation);
            }

            [Test]
            public void NoInnerMemberAccess_ThrowsException()
            {
                var invocation =
                    SyntaxFactory.ParseExpression(@"SomeFunction(""val"", ""col"")") as InvocationExpressionSyntax;

                Assert.Throws<ArgumentException>(() => LocationHelper.GetLocationOfMethodInvocationOnly(invocation));
            }
        }

        [TestFixture]
        public class GetLocationOfWholeInvocationTests
        {
            [Test]
            public void SimpleInvocation()
            {
                var invocation =
                    SyntaxFactory.ParseExpression(@"someClass.MethodA()") as InvocationExpressionSyntax;

                var actualLocation = LocationHelper.GetLocationOfWholeInvocation(invocation);
                var expectedLocation = Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(0, 19));

                Assert.AreEqual(expectedLocation, actualLocation);
            }

            [Test]
            public void MultipleNestedInvocations()
            {
                var invocation =
                    SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().WhereLike(""val"", ""col"")") as InvocationExpressionSyntax;

                var actualLocation = LocationHelper.GetLocationOfWholeInvocation(invocation);
                var expectedLocation = Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(0, 64));

                Assert.AreEqual(expectedLocation, actualLocation);
            }
        }
    }
}