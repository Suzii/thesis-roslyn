using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
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

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateConditionalAccessFormatter();
        }

        [Test]
        public void SimpleConditionalAccess()
        {
            var conditionalAccess = SyntaxFactory.ParseExpression(@"someObject?.PropA") as ConditionalAccessExpressionSyntax;

            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 17));
            
            Assert.AreEqual("someObject?.PropA", _diagnosticFormatter.GetDiagnosedUsage(conditionalAccess));
            Assert.AreEqual(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccess));
        }

        [Test]
        public void ConditionalAccessWithPrecedingMemberAccesses()
        {
            var conditionalAccess = SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or()?.SomeProperty") as ConditionalAccessExpressionSyntax; ;

            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 54));

            Assert.AreEqual(@"new CMS.DataEngine.WhereCondition().Or()?.SomeProperty", _diagnosticFormatter.GetDiagnosedUsage(conditionalAccess));
            Assert.AreEqual(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccess));
        }

        [Test]
        public void ConditionalAccessWithFollowUpMemberAccess()
        {
            var conditionalAccess =
                SyntaxFactory.ParseExpression(@"someObject?.SomeProperty.OtherProperty") as ConditionalAccessExpressionSyntax; ;

            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 24));

            Assert.AreEqual(@"someObject?.SomeProperty", _diagnosticFormatter.GetDiagnosedUsage(conditionalAccess));
            Assert.AreEqual(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccess));
        }

        [Test]
        public void ConditionalAccessWithPrecidingAndFollowUpMemberAccess()
        {
            var conditionalAccess =
                SyntaxFactory.ParseExpression(@"firstObject.someObject?.SomeProperty.OtherProperty") as ConditionalAccessExpressionSyntax; ;
            
            var expectedLocation = Location.Create(conditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, 36));
            Assert.AreEqual(@"firstObject.someObject?.SomeProperty", _diagnosticFormatter.GetDiagnosedUsage(conditionalAccess));
            Assert.AreEqual(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccess));
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

            Assert.AreEqual($@"someObject?.SomeProperty", _diagnosticFormatter.GetDiagnosedUsage(parentConditionalAccess));
            Assert.AreEqual(expectedLocation, _diagnosticFormatter.GetLocation(parentConditionalAccess));
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

            var conditionalAccesss = parentConditionalAccess?.WhenNotNull as ConditionalAccessExpressionSyntax;

            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, precedingDottedExpression.Length + 25));

            Assert.That($@"{precedingDottedExpression}.someObject?.SomeProperty".Contains(_diagnosticFormatter.GetDiagnosedUsage(conditionalAccesss)));
            AssertLocation.IsWithin(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccesss));
        }

        [TestCase("firstObject?.otherObject?")]
        [TestCase("firstObject?.someMethod()?")]
        [TestCase("firstMethod()?.otherMethod()?")]
        [TestCase("firstMethod()?.otherObject?")]
        public void ConditionalAccessWithTwoPrecedingConditionalAccessesAndInvocations(string precedingDottedExpression)
        {
            var parentConditionalAccess =
                SyntaxFactory.ParseExpression($@"{precedingDottedExpression}.someObject?.SomeProperty") as ConditionalAccessExpressionSyntax; ;

            var conditionalAccesss = parentConditionalAccess?.DescendantNodesAndSelf().OfType<ConditionalAccessExpressionSyntax>().ElementAt(1);

            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, precedingDottedExpression.Length + 24));
            
            Assert.That($@"{precedingDottedExpression}.someObject?.SomeProperty".Contains(_diagnosticFormatter.GetDiagnosedUsage(conditionalAccesss)));
            AssertLocation.IsWithin(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccesss));
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

            var conditionalAccesss = parentConditionalAccess?.DescendantNodesAndSelf().OfType<ConditionalAccessExpressionSyntax>().ElementAt(1);
            var expectedLocation = Location.Create(parentConditionalAccess?.SyntaxTree, TextSpan.FromBounds(0, precedingDottedExpression.Length + 28));

            Assert.That($@"{precedingDottedExpression}.someObject?.AccessedMethod".Contains(_diagnosticFormatter.GetDiagnosedUsage(conditionalAccesss)));
            AssertLocation.IsWithin(expectedLocation, _diagnosticFormatter.GetLocation(conditionalAccesss));
        }
    }
}
