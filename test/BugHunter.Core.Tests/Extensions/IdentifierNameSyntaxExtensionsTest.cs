using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace BugHunter.Core.Tests.Extensions
{
    [TestFixture]
    public class IdentifierNameSyntaxExtensionsTest
    {
        class GetOuterMostDottedExpression
        {
            [Test]
            public void SingleIdentifierName_ReturnsSelf()
            {
                var source = @"identifier";

                var expected = SyntaxFactory.IdentifierName(source);
                var actual = expected.GetOuterMostParentOfDottedExpression();

                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void QualifiedName_ReturnsWholeExpression()
            {
                var left = SyntaxFactory.ParseName(@"This.Is.SomeQualified.Name.Syntax");
                var identifierName = SyntaxFactory.IdentifierName(@"AndThisIsSpecificName");

                var expected = SyntaxFactory.QualifiedName(left, identifierName);
                var actual = (expected.Right as IdentifierNameSyntax).GetOuterMostParentOfDottedExpression();

                Assert.AreEqual(expected, actual);
            }


            [Test]
            public void QualifiedNameWithinObjectCreation_ReturnsWholeExpression()
            {
                var objectCreation = SyntaxFactory.ParseExpression("new This.Is.SomeQualified.Name.Syntax.AndThisIsSpecificName()") as ObjectCreationExpressionSyntax;

                var expected = objectCreation.Type as QualifiedNameSyntax;
                var actual = (expected.Right as IdentifierNameSyntax).GetOuterMostParentOfDottedExpression();

                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void MemberAccess_ReturnsWholeExpression()
            {
                var left = SyntaxFactory.ParseName(@"this.is.some.nested.access");
                var identifierName = SyntaxFactory.IdentifierName(@"andThisIsTheAccessedMember");

                var expected = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left, identifierName);
                var actual = (expected.Name as IdentifierNameSyntax).GetOuterMostParentOfDottedExpression();

                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void NestedQualifiedName_ReturnsOuterMostAncestor()
            {
                var left = SyntaxFactory.ParseName(@"This.Is.SomeQualified.Name.Syntax");
                var identifierName = SyntaxFactory.IdentifierName(@"AndThisIsSpecificName");
                var identifierNameSuffix = SyntaxFactory.IdentifierName(@"FinallySomethingElse");

                var nestedQualifiedName = SyntaxFactory.QualifiedName(left, identifierName);
                var expected = SyntaxFactory.QualifiedName(nestedQualifiedName, identifierNameSuffix);
                var actual = ((expected.Left as QualifiedNameSyntax)?.Right as IdentifierNameSyntax).GetOuterMostParentOfDottedExpression();

                Assert.AreEqual(expected, actual);
            }

            [Test]
            public void NestedMemberAccess_ReturnsOuterMostAncestor()
            {
                var left = SyntaxFactory.ParseName(@"this.is.some.nested.access");
                var identifierName = SyntaxFactory.IdentifierName(@"andThisIsTheAccessedMember");
                var identifierNameSuffix = SyntaxFactory.IdentifierName(@"FinallySomethingElse");

                var nestedMemberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left, identifierName);
                var expected = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, nestedMemberAccess, identifierNameSuffix);
                var actual = ((expected.Expression as MemberAccessExpressionSyntax)?.Name as IdentifierNameSyntax).GetOuterMostParentOfDottedExpression();

                Assert.AreEqual(expected, actual);
            }
        }
    }
}