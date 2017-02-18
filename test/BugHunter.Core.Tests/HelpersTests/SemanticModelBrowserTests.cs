using System;
using System.IO;
using System.Linq;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace BugHunter.Core.Tests.HelpersTests
{
    [TestFixture]
    public class SemanticModelBrowserTests
    {
        public class GetNamedTypeSymbol
        {
            [Test]
            public void Null_ThrowsException()
            {
                var syntaxTree = CSharpSyntaxTree.ParseText("");
                var instance = new SemanticModelBrowser(GetCompilation(syntaxTree), syntaxTree);

                Assert.Throws<ArgumentNullException>(() => instance.GetNamedTypeSymbol(null));
            }

            [Test]
            public void QualifiedNameNotType_ReturnsNull()
            {
                var tree = CSharpSyntaxTree.ParseText(@"using System.IO;");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var identifierNameSyntaxes = tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>().ToArray();

                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[0]));
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[1]));
            }

            [Test]
            public void FullyQualifiedIdentifierName_ReturnsCorrectType()
            {
                var tree = CSharpSyntaxTree.ParseText(@"private System.IO.Stream Method() {}");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var identifierNameSyntaxes = tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>().ToArray();

                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[0]));
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[1]));
                Assert.AreEqual("System.IO.Stream", instance.GetNamedTypeSymbol(identifierNameSyntaxes[2]).ToDisplayString());
            }

            [Test]
            public void IdentifierNameInObjectCreationExpression_ReturnsCorrectType()
            {
                var tree = CSharpSyntaxTree.ParseText(@"using System.IO;
public class MyClass
{
    private void Method()
    {
        var a = new BinaryReader(Stream.Null);
    }
}
");
                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var identifierNameSyntaxes = tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>().ToArray();

                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[0]));
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[1]));
                Assert.AreEqual("System.IO.BinaryReader", instance.GetNamedTypeSymbol(identifierNameSyntaxes[2]).ToDisplayString());
                Assert.AreEqual("System.IO.BinaryReader", instance.GetNamedTypeSymbol(identifierNameSyntaxes[3]).ToDisplayString());
                Assert.AreEqual("System.IO.Stream", instance.GetNamedTypeSymbol(identifierNameSyntaxes[4]).ToDisplayString());
                Assert.AreEqual("System.IO.Stream", instance.GetNamedTypeSymbol(identifierNameSyntaxes[5]).ToDisplayString());
            }

            [Test]
            public void FullyQualifiedIdentifierNameInObjectCreationExpression_ReturnsCorrectType()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
public class MyClass
{
    private void Method() {
        var a = new System.IO.BinaryReader(System.IO.Stream.Null);
    }
}
");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var identifierNameSyntaxes = tree.GetRoot().DescendantNodes().OfType<IdentifierNameSyntax>().ToArray();

                Assert.AreEqual("System.IO.BinaryReader", instance.GetNamedTypeSymbol(identifierNameSyntaxes[0]).ToDisplayString());
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[1]));
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[2]));
                Assert.AreEqual("System.IO.BinaryReader", instance.GetNamedTypeSymbol(identifierNameSyntaxes[3]).ToDisplayString());
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[4]));
                Assert.AreEqual(null, instance.GetNamedTypeSymbol(identifierNameSyntaxes[5]));
                Assert.AreEqual("System.IO.Stream", instance.GetNamedTypeSymbol(identifierNameSyntaxes[6]).ToDisplayString());
                Assert.AreEqual("System.IO.Stream", instance.GetNamedTypeSymbol(identifierNameSyntaxes[7]).ToDisplayString());
            }
        }

        private static Compilation GetCompilation(SyntaxTree tree)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { tree }, references: new[] { mscorlib });

            return compilation;
        }
    }
}
