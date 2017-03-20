using System;
using System.Linq;
using BugHunter.Core.AnalysisHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace BugHunter.Core.Tests.AnalysisHelpers
{
    [TestFixture]
    public class AccessExpressionHelperTests
    {
        [TestFixture]
        public class GetTargetTypeTests
        {
            [TestCase(@"""someString"".Length", "string")]
            [TestCase(@"""someString"".Substring(0).Length", "string")]
            [TestCase(@"this.HelperMethod()", "SomeNamespace.SomeClass")]
            [TestCase(@"this.HelperMethod().Length", "string")]
            public void ChainedSimpleMemberAccess_ReturnCorrectType(string accessExpression, string expectedTargetTypeName)
            {
                var compilation = GetCompilationWithAccessExpression(accessExpression);
                var syntaxTree = compilation.SyntaxTrees.First();
                var memberAccess = syntaxTree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .FirstOrDefault();

                var accessExpressionHelper = new AccessExpressionHelper();
                var targetType = accessExpressionHelper.GetTargetType(compilation.GetSemanticModel(syntaxTree),
                    memberAccess);

                Assert.AreEqual(expectedTargetTypeName, targetType.ToDisplayString());
            }


            [TestCase(@"""someString""?.Length", "string")]
            [TestCase(@"""someString""?.Substring(0).Length", "string")]
            [TestCase(@"new SomeClass()?.HelperMethod()", "SomeNamespace.SomeClass")]
            [TestCase(@"this.HelperMethod()?.Length", "string")]
            public void ChainedConditionalAccess_ReturnCorrectType(string accessExpression, string expectedTargetTypeName)
            {

                var a = "adsa"?.Substring(0)?.Length;
                var compilation = GetCompilationWithAccessExpression(accessExpression);
                var syntaxTree = compilation.SyntaxTrees.First();
                var memberAccess = syntaxTree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<ConditionalAccessExpressionSyntax>()
                    .LastOrDefault();

                var accessExpressionHelper = new AccessExpressionHelper();
                var targetType = accessExpressionHelper.GetTargetType(compilation.GetSemanticModel(syntaxTree), memberAccess);

                Assert.AreEqual(expectedTargetTypeName, targetType.ToDisplayString());
            }
        }

        public static Compilation GetCompilationWithAccessExpression(string accessExpression)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText($@"
namespace SomeNamespace 
{{
    public class SomeClass 
    {{
        public string HelperMethod() 
        {{
            return ""Random string"";
            }}

        public void SomeMethod() 
        {{
            {accessExpression};
        }}
    }}
}}");

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                "SomeCompilation",
                syntaxTrees: new[] { syntaxTree },
                references: new[] { mscorlib });

            return compilation;
        }
    }
}
