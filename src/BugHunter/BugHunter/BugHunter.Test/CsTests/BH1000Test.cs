using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using CMS.DataEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class BH1000Test : CodeFixVerifier
    {
        private CodeFixProvider _codeFixProvider = new BH1000CodeFixProvider();

        private DiagnosticAnalyzer _analyzer = new BH1000MethodWhereLikeShouldNotBeUsed();
        
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
        
        [Test]
        public void InputWithWhereLike_SurfacesDiagnostic()
        {
            var dependentTypes = new[] { typeof(WhereConditionBase<>) };

            var test = @"
namespace SampleTestProject.CsSamples
{
    public class BH1000MethodWhereLikeShouldNotBeUsed
    {
        public void SampleMethod()
        {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereLike(""columnName"", ""value"");
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1000",
                Message = "Method WhereLike is used without Architect/CTO approval.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 30)
                        }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"
namespace SampleTestProject.CsSamples
{
    public class BH1000MethodWhereLikeShouldNotBeUsed
    {
        public void SampleMethod()
        {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereContains(""columnName"", ""value"");
        }
    }
}";
            VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithWhereNot_SurfacesDiagnostic()
        {
            var dependentTypes = new[] { typeof(WhereConditionBase<>) };

            var test = @"
namespace SampleTestProject.CsSamples
{
    public class BH1000MethodWhereNotLikeShouldNotBeUsed
    {
        public void SampleMethod()
        {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereNotLike(""columnName"", ""value"");
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1000",
                Message = "Method WhereNotLike is used without Architect/CTO approval.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 30)
                        }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"
namespace SampleTestProject.CsSamples
{
    public class BH1000MethodWhereNotLikeShouldNotBeUsed
    {
        public void SampleMethod()
        {
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.WhereNotContains(""columnName"", ""value"");
        }
    }
}";
            VerifyCSharpFix(test, expectedFix);
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