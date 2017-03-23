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
    public class MemberInvocationOnlyDiagnosticFormatterTests
    {
        private IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter;

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();
        }

        [Test]
        public void SimpleInvocation()
        {
            var invocation = SyntaxFactory.ParseExpression(@"someClass.MethodA()") as InvocationExpressionSyntax;

            var actualLocation = _diagnosticFormatter.GetLocation(invocation);
            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(10, 19));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual("MethodA()", _diagnosticFormatter.GetDiagnosedUsage(invocation));
        }

        [Test]
        public void MultipleNestedInvocations()
        {
            var invocation =
                SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().WhereLike(""val"", ""col"")") as InvocationExpressionSyntax;

            var actualLocation = _diagnosticFormatter.GetLocation(invocation);
            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(41, 64));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual(@"WhereLike(""val"", ""col"")", _diagnosticFormatter.GetDiagnosedUsage(invocation));
        }

        [Test]
        public void NoInnerMemberAccess_ThrowsException()
        {
            var invocation =
                SyntaxFactory.ParseExpression(@"SomeFunction(""val"", ""col"")") as InvocationExpressionSyntax;

            Assert.Throws<ArgumentException>(() => _diagnosticFormatter.GetLocation(invocation));
        }
    }
}
