using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class BH1000Test : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new BH1000CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new BH1000MethodWhereLikeShouldNotBeUsed();
        }

        protected override MetadataReference[] GetAdditionalReferences()
        {
            return Constants.BasicReferences;
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithWhereLike_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class BH1000MethodWhereLikeShouldNotBeUsed
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{oldMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1000",
                Message = $"Method {oldMethodCall} is used without Architect/CTO approval.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 30)
                        }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class BH1000MethodWhereLikeShouldNotBeUsed
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{newMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [Test]
        public void InputWithPossibleFalsePositive_NoDiagnostic()
        {
            var test = @"
namespace SampleTestProject.CsSamples
{
    public class BH1000MethodWhereLikeShouldNotBeUsed
    {
        public void WhereLike() {
            // do nothing
        }

        public void FalsePositiveForWhereLike()
        {
            WhereLike();
            this.WhereLike();
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }
    }
}