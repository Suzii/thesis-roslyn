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
    public class MemberAccessDiagnosticFormatterTests
    {
        private IDiagnosticFormatter _diagnosticFormatter;

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberAccessFormatter();
        }

        [Test]
        public void SimpleMemberAccess()
        {
            var memberAccess = SyntaxFactory.ParseExpression(@"someClass.PropA");

            var actualLocation = _diagnosticFormatter.GetLocation(memberAccess);
            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(0, 15));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual("someClass.PropA", _diagnosticFormatter.GetDiagnosedUsage(memberAccess));
        }

        [Test]
        public void MultipleNestedMemberAccesses()
        {
            var memberAccess =
                SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().SomeProperty");

            var actualLocation = _diagnosticFormatter.GetLocation(memberAccess);
            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(0, 53));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual(@"new CMS.DataEngine.WhereCondition().Or().SomeProperty", _diagnosticFormatter.GetDiagnosedUsage(memberAccess));
        }

        [Test]
        public void NoInnerMemberAccess_ThrowsException()
        {
            var memberAccess =
                SyntaxFactory.ParseExpression(@"SomeFunction(""val"", ""col"")");

            Assert.Throws<ArgumentException>(() => _diagnosticFormatter.GetLocation(memberAccess));
            Assert.Throws<ArgumentException>(() => _diagnosticFormatter.GetDiagnosedUsage(memberAccess));
        }
    }
}
