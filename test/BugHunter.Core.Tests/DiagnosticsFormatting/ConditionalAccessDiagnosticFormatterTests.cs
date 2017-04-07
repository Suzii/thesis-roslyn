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
    public class ConditionalAccessDiagnosticFormatterTests
    {
        private IDiagnosticFormatter<ConditionalAccessExpressionSyntax> _diagnosticFormatter;
        private readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor("FakeID", "Title", "{0}", "Category", DiagnosticSeverity.Warning, true);

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = new ConditionalAccessDiagnosticFormatter();
        }
        
        [Test]
        public void SimpleConditionalAccess()
        {
            var conditionalAccess = SyntaxFactory.ParseExpression(@"someObject?.PropA") as ConditionalAccessExpressionSyntax;

            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 17));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.AreEqual("someObject?.PropA", diagnostic.GetMessage());
            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void ConditionalAccessWithPrecedingMemberAccesses()
        {
            var conditionalAccess = SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or()?.SomeProperty") as ConditionalAccessExpressionSyntax; ;

            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 54));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.AreEqual(@"new CMS.DataEngine.WhereCondition().Or()?.SomeProperty", diagnostic.GetMessage());
            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void ConditionalAccessWithFollowUpMemberAccess()
        {
            var conditionalAccess =
                SyntaxFactory.ParseExpression(@"someObject?.SomeProperty.OtherProperty") as ConditionalAccessExpressionSyntax; ;

            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 24));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.AreEqual(@"someObject?.SomeProperty", diagnostic.GetMessage());
            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void ConditionalAccessWithPrecedingAndFollowUpMemberAccess()
        {
            var conditionalAccess =
                SyntaxFactory.ParseExpression(@"firstObject.someObject?.SomeProperty.OtherProperty") as ConditionalAccessExpressionSyntax; ;
            
            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 36));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.AreEqual(@"firstObject.someObject?.SomeProperty", diagnostic.GetMessage());
            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [TestCase(".OtherProperty")]
        [TestCase("?.OtherProperty")]
        [TestCase(".OtherMethod()")]
        [TestCase("?.OtherMethod()")]
        [TestCase(".OtherProperty.LastOne")]
        [TestCase("?.OtherProperty?.LastOne()")]
        [TestCase(".OtherProperty.LastOne()")]
        [TestCase("?.OtherProperty?.LastOne()")]
        [TestCase(".OtherMethod().LastOne")]
        [TestCase("?.OtherMethod()?.LastOne")]
        [TestCase(".OtherMethod().LastOne()")]
        [TestCase("?.OtherMethod()?.LastOne()")]
        public void ConditionalAccessWithFollowUpConditionalAccesses(string followUpDottedExpression)
        {
            var parentConditionalAccess =
                SyntaxFactory.ParseExpression($@"someObject?.SomeProperty{followUpDottedExpression}") as ConditionalAccessExpressionSyntax; ;

            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 24));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, parentConditionalAccess);

            Assert.AreEqual(@"someObject?.SomeProperty", diagnostic.GetMessage());
            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [TestCase("firstObject?")]
        [TestCase("someMethod()?.otherObject")]
        [TestCase("someMethod().otherObject?")]
        [TestCase("firstObject?.someMethod()")]
        [TestCase("firstObject.someMethod()?")]
        public void ConditionalAccessWithPrecedingConditionalAccessesAndInvocations(string precedingDottedExpression)
        {
            var parentConditionalAccess =
                SyntaxFactory.ParseExpression($@"{precedingDottedExpression}.someObject?.SomeProperty") as ConditionalAccessExpressionSyntax; ;

            var conditionalAccess = parentConditionalAccess?.WhenNotNull as ConditionalAccessExpressionSyntax;

            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, precedingDottedExpression.Length + 25));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.That($@"{precedingDottedExpression}.someObject?.SomeProperty".Contains(diagnostic.GetMessage()));
            AssertLocation.IsWithin(expectedLocation, diagnostic.Location);
            Assert.IsTrue(diagnostic.IsMarkedAsConditionalAccess());
        }

        [TestCase("firstObject?.otherObject?")]
        [TestCase("firstObject?.someMethod()?")]
        [TestCase("firstMethod()?.otherMethod()?")]
        [TestCase("firstMethod()?.otherObject?")]
        public void ConditionalAccessWithTwoPrecedingConditionalAccessesAndInvocations(string precedingDottedExpression)
        {
            var parentConditionalAccess =
                SyntaxFactory.ParseExpression($@"{precedingDottedExpression}.someObject?.SomeProperty") as ConditionalAccessExpressionSyntax; ;

            var conditionalAccess = parentConditionalAccess?.DescendantNodesAndSelf().OfType<ConditionalAccessExpressionSyntax>().ElementAt(1);

            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, precedingDottedExpression.Length + 24));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.That($@"{precedingDottedExpression}.someObject?.SomeProperty".Contains(diagnostic.GetMessage()));
            AssertLocation.IsWithin(expectedLocation, diagnostic.Location);
        }

        [TestCase("firstObject?")]
        [TestCase("someMethod()?.otherObject")]
        [TestCase("someMethod().otherObject?")]
        [TestCase("firstObject?.someMethod()")]
        [TestCase("firstObject.someMethod()?")]
        public void ConditionalAccessWithInvocation_ReportsCorrectly(string precedingDottedExpression)
        {
            var parentConditionalAccess =
                SyntaxFactory.ParseExpression($@"{precedingDottedExpression}.someObject?.AccessedMethod().Other") as ConditionalAccessExpressionSyntax; ;

            var conditionalAccess = parentConditionalAccess?.DescendantNodesAndSelf().OfType<ConditionalAccessExpressionSyntax>().ElementAt(1);
            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, precedingDottedExpression.Length + 28));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, conditionalAccess);

            Assert.That($@"{precedingDottedExpression}.someObject?.AccessedMethod".Contains(diagnostic.GetMessage()));
            AssertLocation.IsWithin(expectedLocation, diagnostic.Location);
        }
    }
}
