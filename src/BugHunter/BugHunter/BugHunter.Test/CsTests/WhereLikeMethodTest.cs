using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Kentico.Google.Apis.Util;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class WhereLikeMethodTest : CodeFixVerifier<WhereLikeMethodAnalyzer, WhereLikeMethodCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences;
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
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.WHERE_LIKE_METHOD,
                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($@"whereCondition.{oldMethodCall}(""columnName"", ""value"")"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 45) }
            };

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
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.WHERE_LIKE_METHOD,
                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($@"whereCondition.{oldMethodCall}(""columnName"", ""value"")"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 45) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var whereCondition = new CMS.DataEngine.WhereCondition();
            whereCondition = whereCondition.{newMethodCall}(""columnName"", ""value"").WhereTrue(""this is gonna be tricky"");
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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.WHERE_LIKE_METHOD,
                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($@"whereCondition.Or().{oldMethodCall}(""columnName"", ""value"")"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 50) }
            };

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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.WHERE_LIKE_METHOD,
                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($@"new CMS.DataEngine.WhereCondition().{oldMethodCall}(""columnName"", ""value"")"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 70) }
            };

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