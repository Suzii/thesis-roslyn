using System;
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
        public class GetMemberAccessTarget
        {
            [Test]
            public void SimpleLiteral()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
	public class MyClass
    {
        public void Main()
        {
            var something = ""something"".Trim();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("string", actual.ToDisplayString());
            }

            [Test]
            public void IdentifierName()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
	public class MyClass
    {
        private string _sth = ""something"";

        public void Main()
        {
            var something = _sth.Trim();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("string", actual.ToDisplayString());
            }

            [Test]
            public void PredefinedType()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
	public class MyClass
    {
        private string _sth = ""something"";

        public void Main()
        {
            var something = string.Equals(""a"", ""a"");
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("string", actual.ToDisplayString());
            }

            [Test]
            public void InvocationExpression()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
	public class MyClass
    {
        private string GetString() {
            return ""whatever"";
        }

        public void Main()
        {
            var something = GetString().Trim();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("string", actual.ToDisplayString());
            }

            [Test]
            public void ThisExpression()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
	public class MyClass
    {
        private string GetString() {
            return ""whatever"";
        }

        public void Main()
        {
            var something = this.GetString();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("MyClass", actual.ToDisplayString());
            }

            [Test]
            public void SuperExpression()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
    public class SuperClass {
        protected virtual string GetString() {
            return ""whatever"";
        }
    }

	public class MyClass : SuperClass
    {
        protected override string GetString() {
            return ""something else"";
        }

        public void Main()
        {
            var something = base.GetString();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());
                Assert.AreEqual("SuperClass", actual.ToDisplayString());
            }

            [Test]
            public void ObjectCreationExpression()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
	public class MyClass
    {
        private string GetString() {
            return ""whatever"";
        }

        public void Main()
        {
            var something = new MyClass().GetString();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("MyClass", actual.ToDisplayString());
            }

            [Test]
            public void MemberAccessExpression()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
    public class A {
        public string Sth = ""something"";
    }

	public class MyClass
    {
        public void Main()
        {
            var something = new A().Sth.GetString();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("string", actual.ToDisplayString());
            }

            [Test]
            public void ConditionalMemberAccessExpression()
            {
                var tree = CSharpSyntaxTree.ParseText(@"
    public class A {
        public string Sth = ""something"";
    }

	public class MyClass
    {
        public void Main()
        {
            var something = new A()?.Sth.GetString();
        }
    }");

                var instance = new SemanticModelBrowser(GetCompilation(tree), tree);
                var memberAccessExpressionSyntaxes = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
                var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxes.FirstOrDefault());

                Assert.AreEqual("string", actual.ToDisplayString());
            }
        }

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
