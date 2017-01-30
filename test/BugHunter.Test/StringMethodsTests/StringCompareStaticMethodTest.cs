using System;
using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.StringMethodsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringCompareStaticMethodTest : CodeFixVerifier<StringCompareStaticMethodAnalyzer, StringCompareStaticMethodCodeFixProvider>
    {
        static readonly object[] TestSource =
        {
            new object[] { @"Compare(""a"", ""aa"")", @"Compare(""a"", ""aa"", StringComparison.CurrentCulture)", 0 },
            new object[] { @"Compare(""a"", ""aa"")", @"Compare(""a"", ""aa"", StringComparison.CurrentCultureIgnoreCase)", 1 },
            new object[] { @"Compare(""a"", ""aa"")", @"Compare(""a"", ""aa"", StringComparison.InvariantCulture)", 2 },
            new object[] { @"Compare(""a"", ""aa"")", @"Compare(""a"", ""aa"", StringComparison.InvariantCultureIgnoreCase)", 3 },

            new object[] { @"Compare(""a"", ""aa"", true)", @"Compare(""a"", ""aa"", StringComparison.CurrentCultureIgnoreCase)", 0 },
            new object[] { @"Compare(""a"", ""aa"", true)", @"Compare(""a"", ""aa"", StringComparison.InvariantCultureIgnoreCase)", 1 },
            new object[] { @"Compare(""a"", ""aa"", false)", @"Compare(""a"", ""aa"", StringComparison.CurrentCulture)", 0 },
            new object[] { @"Compare(""a"", ""aa"", false)", @"Compare(""a"", ""aa"", StringComparison.InvariantCulture)", 1 },

            new object[] { @"Compare(""a"", 0, ""aa"", 0, 1, true)", @"Compare(""a"", 0, ""aa"", 0, 1, StringComparison.CurrentCultureIgnoreCase)", 0 },
            new object[] { @"Compare(""a"", 0, ""aa"", 0, 1, true)", @"Compare(""a"", 0, ""aa"", 0, 1, StringComparison.InvariantCultureIgnoreCase)", 1 },
            new object[] { @"Compare(""a"", 0, ""aa"", 0, 1, false)", @"Compare(""a"", 0, ""aa"", 0, 1, StringComparison.CurrentCulture)", 0 },
            new object[] { @"Compare(""a"", 0, ""aa"", 0, 1, false)", @"Compare(""a"", 0, ""aa"", 0, 1, StringComparison.InvariantCulture)", 1 },
        };

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

        [TestCase(@"Compare(""a"", ""b"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"Compare(""a"", ""b"", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase)")]
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

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_COMPARE_STATIC_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 33) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = string.{codeFix};
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = string.{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_COMPARE_STATIC_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 33) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = string.{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [TestCase(@"Compare(""a"", ""b"", false)")]
        [TestCase(@"Compare(""a"", ""b"", true)")]
        [TestCase(@"Compare(""a"", 0, ""aa"", 0, 1, true)")]
        [TestCase(@"Compare(""a"", 0, ""aa"", 0, 1, false)")]
        public void InputWithIncident_IgnoreCaseParam_AndProvidesOnlyAppropriateFixes(string methodUsed)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = string.{methodUsed}.ToString();
        }}
    }}
}}";
            Assert.Throws<ArgumentOutOfRangeException>(() => VerifyCSharpFix(test, test, 2));
        }
    }
}