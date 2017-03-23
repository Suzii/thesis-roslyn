using System;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    [TestFixture]
    public class MemberAccessOnlyDiagnosticFormatterTests
    {
        private IDiagnosticFormatter<MemberAccessExpressionSyntax> _diagnosticFormatter;

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberAccessOnlyFormatter();
        }

        [Test]
        public void SimpleMemberAccess()
        {
            var memberAccess = SyntaxFactory.ParseExpression(@"someClass.PropA") as MemberAccessExpressionSyntax;

            var actualLocation = _diagnosticFormatter.GetLocation(memberAccess);
            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(10, 15));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual("PropA", _diagnosticFormatter.GetDiagnosedUsage(memberAccess));
        }

        [Test]
        public void MultipleNestedMemberAccesses()
        {
            var memberAccess = SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().SomeProperty") as MemberAccessExpressionSyntax;

            var actualLocation = _diagnosticFormatter.GetLocation(memberAccess);
            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(41, 53));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual(@"SomeProperty", _diagnosticFormatter.GetDiagnosedUsage(memberAccess));
        }
    }
}
