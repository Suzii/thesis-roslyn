using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringEqualsAndCompareStaticTest : CodeFixVerifier<StringEqualsAndCompareStaticMethodsAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return null;
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"Equals(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Compare(""a"", ""b"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"Compare(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Compare(""a"", 0, ""b"", 1, 1, false, CultureInfo.CurrentCulture)")]
        [TestCase(@"Compare(""a"", 0, ""b"", 1, 1, StringComparison.InvariantCultureIgnoreCase)")]
        public void AllowedOverloadCalled_NoDiagnostic(string methodUsed)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = string.{methodUsed};
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"Equals(""a"", ""a"")", @"Equals(""a"", ""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Compare(""a"", ""b"")", @"Compare(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Compare(string1, 0, string2, 0, 5)", "// TODO")]
        [TestCase(@"Compare(string1, 0, string2, 0, 5, false)", "// TODO")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = string.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_EQUALS_COMPARE_STATIC_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 33) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{codeFix};
        }}
    }}
}}";
            //VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"Equals(""a"", ""a"")", @"Equals(""a"", ""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Compare(""a"", ""b"")", @"Compare(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Compare(string1, 0, string2, 0, 5)", "// TODO")]
        [TestCase(@"Compare(string1, 0, string2, 0, 5, false)", "// TODO")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = string.{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_EQUALS_COMPARE_STATIC_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 33) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{codeFix};
        }}
    }}
}}";
            //VerifyCSharpFix(test, expectedFix);
        }
    }
}