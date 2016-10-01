using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using CMS.DataEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BugHunter.Test.CsTests
{
    [TestClass]
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

        [TestMethod]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestMethod]
        public void InputWithOneIncident_SurfacesDiagnostic()
        {
            var dependentTypes = new[] {typeof(WhereConditionBase<>)};

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
            var expected = new DiagnosticResult
            {
                Id = "BH1000",
                Message = "Method WhereLike is used without Architect/CTO approval.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 30)
                        }
            };

            VerifyCSharpDiagnostic(test, dependentTypes, expected);

            var fixtest = @"
    namespace SampleTestProject.CsSamples
    {
        public class BH1000MethodWhereLikeShouldNotBeUsed
        {
            public void SampleMethod()
            {
                var whereCondition = new CMS.DataEngine.WhereCondition();
                whereCondition = whereCondition.Contains(""columnName"", ""value"");
            }
        }
    }";
            VerifyCSharpFix(test, fixtest, dependentTypes);
        }

        [TestMethod]
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