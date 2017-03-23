using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    [TestFixture]
    public class MemberInvocationDiagnosticFormatterTests
    {
        private IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter;

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationFormatter();
        }

        [Test]
        public void SimpleInvocation()
        {
            var invocation = SyntaxFactory.ParseExpression(@"someClass.MethodA()") as InvocationExpressionSyntax;

            var actualLocation = _diagnosticFormatter.GetLocation(invocation);
            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(0, 19));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual(@"someClass.MethodA()", _diagnosticFormatter.GetDiagnosedUsage(invocation));
        }

        [Test]
        public void MultipleNestedInvocations()
        {
            var invocation =
                SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().WhereLike(""val"", ""col"")") as InvocationExpressionSyntax;

            var actualLocation = _diagnosticFormatter.GetLocation(invocation);
            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(0, 64));

            Assert.AreEqual(expectedLocation, actualLocation);

            Assert.AreEqual(@"new CMS.DataEngine.WhereCondition().Or().WhereLike(""val"", ""col"")", _diagnosticFormatter.GetDiagnosedUsage(invocation));
        }
    }
}
