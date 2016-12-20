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
        [Test]
        public void GetMemberAccessTarget_SimpleLiteral()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("string", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_IdentifierName()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("string", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_PredefinedType()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("string", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_InvocationExpression()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("string", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_ThisExpression()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("MyClass", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_SuperExpression()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());
            Assert.AreEqual("SuperClass", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_ObjectCreationExpression()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("MyClass", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_MemberAccessExpression()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("string", actual.ToDisplayString());
        }

        [Test]
        public void GetMemberAccessTarget_ConditionalMemberAccessExpression()
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
            var memberAccessExpressionSyntaxs = tree.GetRoot().DescendantNodes().OfType<MemberAccessExpressionSyntax>();
            var actual = instance.GetMemberAccessTarget(memberAccessExpressionSyntaxs.FirstOrDefault());

            Assert.AreEqual("string", actual.ToDisplayString());
        }


        private static Compilation GetCompilation(SyntaxTree tree)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation", syntaxTrees: new[] { tree }, references: new[] { mscorlib });

            return compilation;
        }
    }
}
