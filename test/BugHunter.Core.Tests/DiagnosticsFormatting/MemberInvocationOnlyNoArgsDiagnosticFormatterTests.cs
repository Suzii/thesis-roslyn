using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace BugHunter.Core.Tests.DiagnosticsFormatting
{
    [TestFixture]
    public class MemberInvocationOnlyNoArgsDiagnosticFormatterTests
    {
        private IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter;
        private readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor("FakeID", "Title", "{0}", "Category", DiagnosticSeverity.Warning, true);

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = new MemberInvocationOnlyNoArgsDiagnosticFormatter();
        }

        [Test]
        public void SimpleInvocation()
        {
            var invocation = SyntaxFactory.ParseExpression(@"someClass.MethodA()") as InvocationExpressionSyntax;

            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(10, 19));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, invocation);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual("MethodA()", diagnostic.GetMessage());
            Assert.IsTrue(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsFalse(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void MultipleNestedInvocations()
        {
            var invocation =
                SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().WhereLike(""val"", ""col"")") as InvocationExpressionSyntax;

            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(41, 64));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, invocation);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual(@"WhereLike()", diagnostic.GetMessage());
            Assert.IsTrue(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsFalse(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void SimpleConditionalInvocation()
        {
            var expression = SyntaxFactory.ParseExpression(@"someClass?.MethodA()");
            var invocation = expression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().First();

            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(11, 20));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, invocation);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual(@"MethodA()", diagnostic.GetMessage());
            Assert.IsFalse(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void MultipleNestedConditionalInvocations()
        {
            var expression = SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or()?.WhereLike(""val"", ""col"")");
            var invocation = expression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Last();

            var expectedLocation = Location.Create(invocation?.SyntaxTree, TextSpan.FromBounds(42, 65));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, invocation);

            AssertLocation.IsWithin(expectedLocation, diagnostic.Location);
            Assert.That(@"WhereLike()".Contains(diagnostic.GetMessage()));
            Assert.IsFalse(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }
    }
}
