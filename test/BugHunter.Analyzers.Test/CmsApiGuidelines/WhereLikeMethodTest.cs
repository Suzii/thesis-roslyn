using BugHunter.Analyzers.CmsApiGuidelinesRules.Analyzers;
using BugHunter.Analyzers.CmsApiGuidelinesRules.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiGuidelines
{
    [TestFixture]
    public class WhereLikeMethodTest : CodeFixVerifier<WhereLikeMethodAnalyzer, WhereLikeMethodCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences;

        private static DiagnosticResult CreateDiagnosticResult(string usage)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.WHERE_LIKE_METHOD,
                Message = string.Format(MessagesConstants.MESSAGE_NO_SUGGESTION, usage),
                Severity = DiagnosticSeverity.Warning,
            };


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
        public void InputWithWhereLike_SimpleMemberAccess_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{oldMethodCall}(""columnName"", ""value"");
            //whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")").WithLocation(9, 45);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{newMethodCall}(""columnName"", ""value"");
            //whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithWhereLike_FollowUpMemberAccess_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{oldMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
            //whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
            //whereCondition = whereCondition.{oldMethodCall}(""columnName"", ""value"")?.WhereTrue(""this is gonna be tricky"");
            //whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"")?.WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")");

            VerifyCSharpDiagnostic(test, 
                expectedDiagnostic.WithLocation(9, 45)
                //expectedDiagnostic.WithLocation(10, 45),
                //expectedDiagnostic.WithLocation(11, 45),
                //expectedDiagnostic.WithLocation(12, 45)
                );

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{newMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
            //whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
            //whereCondition = whereCondition.{oldMethodCall}(""columnName"", ""value"")?.WhereTrue(""this is gonna be tricky"");
            //whereCondition = whereCondition?.{oldMethodCall}(""columnName"", ""value"")?.WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithWhereLike_PrecedingMemberAccess_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.Or().{oldMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")").WithLocation(9, 50);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.Or().{newMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixIndex);
        }

        [TestCase("WhereLike", "WhereContains", 0)]
        [TestCase("WhereLike", "WhereStartsWith", 1)]
        [TestCase("WhereLike", "WhereEndsWith", 2)]
        [TestCase("WhereNotLike", "WhereNotContains", 0)]
        [TestCase("WhereNotLike", "WhereNotStartsWith", 1)]
        [TestCase("WhereNotLike", "WhereNotEndsWith", 2)]
        public void InputWithFullyQualifiedWhereLike_SurfacesDiagnostic(string oldMethodCall, string newMethodCall, int codeFixIndex)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition().{oldMethodCall}(""columnName"", ""value"");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{oldMethodCall}(""columnName"", ""value"")").WithLocation(8, 70);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition().{newMethodCall}(""columnName"", ""value"");
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
    public class SampleClass
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