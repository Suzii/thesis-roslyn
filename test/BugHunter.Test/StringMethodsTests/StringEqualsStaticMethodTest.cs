using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.StringMethodsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringEqualsStaticMethodTest : CodeFixVerifier<StringEqualsMethodAnalyzer, StringComparisonMethodsWithModifierCodeFixProvider>
    {
        static readonly object[] TestSource =
        {
            new object[] { @"Equals(""a"", ""b"")", @"Equals(""a"", ""b"", StringComparison.CurrentCulture)", 0 },
            new object[] { @"Equals(""a"", ""b"")", @"Equals(""a"", ""b"", StringComparison.CurrentCultureIgnoreCase)", 1 },
            new object[] { @"Equals(""a"", ""b"")", @"Equals(""a"", ""b"", StringComparison.InvariantCulture)", 2 },
            new object[] { @"Equals(""a"", ""b"")", @"Equals(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)", 3 },
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

        [TestCase(@"Equals(""a"", ""b"", StringComparison.InvariantCultureIgnoreCase)")]
        public void AllowedOverloadCalled_NoDiagnostic(string methodUsed)
        {
            var test = $@"using System;
using System.Globalization;

namespace SampleTestProject.CsSamples 
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
                Id = DiagnosticIds.STRING_EQUALS_METHOD,
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
                Id = DiagnosticIds.STRING_EQUALS_METHOD,
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
    }
}