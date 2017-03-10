using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Analyzers.StringAndCultureRules.CodeFixes;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.StringAndCultureTests
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

        protected override MetadataReference[] GetAdditionalReferences() => null;


        private DiagnosticResult GetDiagnosticResult(string methodUsed)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_EQUALS_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"Equals(""a"", ""b"", sc)")]
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
            var sc = StringComparison.InvariantCultureIgnoreCase;
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


            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(7, 33);
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


            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(7, 33);
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