// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace BugHunter.Core.Tests.Extensions
{
    [TestFixture]
    public class ClassDeclarationSyntaxExtenstionsTest
    {
        private static ClassDeclarationSyntax GetClassDeclarationSyntax(string classDeclaration)
        {
            return SyntaxFactory
                .ParseSyntaxTree(classDeclaration)
                .GetRoot()
                .DescendantNodesAndSelf()
                .OfType<ClassDeclarationSyntax>()
                .Single();
        }

        private class IsPublic
        {
            [TestCase("public class A { }")]
            [TestCase("public abstract class A { }")]
            [TestCase("public partial class A { }")]
            public void PublicClass_IsPublic_ReturnsTrue(string classDeclaration)
            {
                Assert.IsTrue(GetClassDeclarationSyntax(classDeclaration).IsPublic());
            }

            [TestCase("class A { }")]
            [TestCase("internal class A { }")]
            [TestCase("private class A { }")]
            public void NonPublicClass_IsPublic_ReturnsFalse(string classDeclaration)
            {
                Assert.IsFalse(GetClassDeclarationSyntax(classDeclaration).IsPublic());
            }
        }

        private class IsPartial
        {
            [TestCase("partial class A { }")]
            [TestCase("internal partial class A { }")]
            [TestCase("private partial class A { }")]
            [TestCase("public abstract partial class A { }")]
            public void PartialClass_IsPartial_ReturnsTrue(string classDeclaration)
            {
                Assert.IsTrue(GetClassDeclarationSyntax(classDeclaration).IsPartial());
            }

            [TestCase("class A { }")]
            [TestCase("internal abstract class A { }")]
            [TestCase("private class A { }")]
            [TestCase("public abstract class A { }")]
            public void NonPartialClass_IsPartial_ReturnsFalse(string classDeclaration)
            {
                Assert.IsFalse(GetClassDeclarationSyntax(classDeclaration).IsPartial());
            }
        }

        private class IsAbstract
        {
            [TestCase("abstract class A { }")]
            [TestCase("internal abstract class A { }")]
            [TestCase("private abstract class A { }")]
            [TestCase("public abstract partial class A { }")]
            public void AbstractClass_IsAbstract_ReturnsTrue(string classDeclaration)
            {
                Assert.IsTrue(GetClassDeclarationSyntax(classDeclaration).IsAbstract());
            }

            [TestCase("class A { }")]
            [TestCase("internal class A { }")]
            [TestCase("private class A { }")]
            [TestCase("public partial class A { }")]
            public void NonAbstractClass_IsAbstract_ReturnsFalse(string classDeclaration)
            {
                Assert.IsFalse(GetClassDeclarationSyntax(classDeclaration).IsAbstract());
            }
        }

        private class WithBaseType
        {
            [Test]
            public void NoBaseList_AddsBaseType()
            {
                var test = $"public class SampleClass\r\n{{\r\n}}";
                var expectedFix = $"public class SampleClass : SomeBaseClass\r\n{{\r\n}}";

                Assert.AreEqual(expectedFix, GetClassDeclarationSyntax(test).WithBaseClass("SomeBaseClass").ToString());
            }

            [Test]
            public void HasBaseType_ReplacesBaseType()
            {
                var test = $"public class SampleClass : BaseTypeToBeReplaced\r\n{{\r\n}}";
                var expectedFix = $"public class SampleClass : SomeBaseClass\r\n{{\r\n}}";

                Assert.AreEqual(expectedFix, GetClassDeclarationSyntax(test).WithBaseClass("SomeBaseClass").ToString());
            }

            [Test]
            public void ImplementsInterface_AddsBaseType()
            {
                var test = $"public class SampleClass : IRandomInterface\r\n{{\r\n}}";
                var expectedFix = $"public class SampleClass : SomeBaseClass, IRandomInterface\r\n{{\r\n}}";

                Assert.AreEqual(expectedFix, GetClassDeclarationSyntax(test).WithBaseClass("SomeBaseClass").ToString());
            }

            [Test]
            public void ImplementsFullyQualifiedInterface_AddsBaseType()
            {
                var test = $"public class SampleClass : Some.Namespace.IRandomInterface\r\n{{\r\n}}";
                var expectedFix = $"public class SampleClass : SomeBaseClass, Some.Namespace.IRandomInterface\r\n{{\r\n}}";

                Assert.AreEqual(expectedFix, GetClassDeclarationSyntax(test).WithBaseClass("SomeBaseClass").ToString());
            }

            [Test]
            public void ImplementsInterfaceAndExtendsBaseType_ReplacesBaseType()
            {
                var test = $"public class SampleClass : BaseTypeToBeReplaced, IRandomInterface\r\n{{\r\n}}";
                var expectedFix = $"public class SampleClass : SomeBaseClass, IRandomInterface\r\n{{\r\n}}";

                Assert.AreEqual(expectedFix, GetClassDeclarationSyntax(test).WithBaseClass("SomeBaseClass").ToString());
            }

            [Test]
            public void ImplementsFullyQualifiedInterfaceAndExtendsBaseType_ReplacesBaseType()
            {
                var test = $"public class SampleClass : BaseTypeToBeReplaced, Some.Namespace.IRandomInterface\r\n{{\r\n}}";
                var expectedFix = $"public class SampleClass : SomeBaseClass, Some.Namespace.IRandomInterface\r\n{{\r\n}}";

                Assert.AreEqual(expectedFix, GetClassDeclarationSyntax(test).WithBaseClass("SomeBaseClass").ToString());
            }
        }
    }
}