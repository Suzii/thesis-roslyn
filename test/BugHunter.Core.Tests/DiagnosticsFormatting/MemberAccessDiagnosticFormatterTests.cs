// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    public class MemberAccessDiagnosticFormatterTests
    {
        private readonly DiagnosticDescriptor _rule = new DiagnosticDescriptor("FakeID", "Title", "{0}", "Category", DiagnosticSeverity.Warning, true);
        private ISyntaxNodeDiagnosticFormatter<MemberAccessExpressionSyntax> _diagnosticFormatter;

        [SetUp]
        public void SetUp()
        {
            _diagnosticFormatter = new MemberAccessDiagnosticFormatter();
        }

        [Test]
        public void SimpleMemberAccess()
        {
            var memberAccess = SyntaxFactory.ParseExpression(@"someClass.PropA") as MemberAccessExpressionSyntax;

            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(0, 15));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, memberAccess);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual("someClass.PropA", diagnostic.GetMessage());
            Assert.IsTrue(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsFalse(diagnostic.IsMarkedAsConditionalAccess());
        }

        [Test]
        public void MultipleNestedMemberAccesses()
        {
            var memberAccess =
                SyntaxFactory.ParseExpression(@"new CMS.DataEngine.WhereCondition().Or().SomeProperty") as MemberAccessExpressionSyntax;

            var expectedLocation = Location.Create(memberAccess?.SyntaxTree, TextSpan.FromBounds(0, 53));
            var diagnostic = _diagnosticFormatter.CreateDiagnostic(_rule, memberAccess);

            Assert.AreEqual(expectedLocation, diagnostic.Location);
            Assert.AreEqual(@"new CMS.DataEngine.WhereCondition().Or().SomeProperty", diagnostic.GetMessage());
            Assert.IsTrue(diagnostic.IsMarkedAsSimpleMemberAccess());
            Assert.IsFalse(diagnostic.IsMarkedAsConditionalAccess());
        }
    }
}
