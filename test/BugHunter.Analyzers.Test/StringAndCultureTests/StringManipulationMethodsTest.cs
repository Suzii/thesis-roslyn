using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Analyzers.StringAndCultureRules.CodeFixes;
using BugHunter.Core.Constants;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.StringAndCultureTests
{
    [TestFixture]
    public class StringManipulationMethodsTest : CodeFixVerifier<StringManipulationMethodsAnalyzer, StringManipulationMethodsCodeFixProvider>
    {
        private static readonly object[] TestSource =
        {
            new object[] { "ToLower()", "ToLowerInvariant()", null, 0 },
            new object[] { "ToLower()", "ToLower(CultureInfo.CurrentCulture)", "using System.Globalization;\r\n\r\n", 1 },

            new object[] { "ToUpper()", "ToUpperInvariant()", null, 0 },
            new object[] { "ToUpper()", "ToUpper(CultureInfo.CurrentCulture)", "using System.Globalization;\r\n\r\n", 1 },
        };

        protected override MetadataReference[] GetAdditionalReferences() => null;

        private DiagnosticResult GetDiagnosticResult(string methodUsed)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.StringManipulationMethods,
                Message = $"'{methodUsed}' used without specifying CultureInfo.",
                Severity = DiagnosticSeverity.Warning,
            };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("ToLowerInvariant()")]
        [TestCase("ToLower(CultureInfo.CurrentCulture)")]
        [TestCase("ToLower(ci)")]
        [TestCase("ToLower(CultureInfo.InvariantCulture)")]
        [TestCase("ToUpperInvariant()")]
        [TestCase("ToUpper(CultureInfo.CurrentCulture)")]
        [TestCase("ToUpper(CultureInfo.InvariantCulture)")]
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
            var original = ""Original string"";
            var ci = CultureInfo.CurrentCulture;
            var updated1 = original.{methodUsed}.ToString();
            var updated2 = original?.{methodUsed}.ToString();
            var updated3 = original.{methodUsed}?.ToString();
            var updated4 = original?.{methodUsed}?.ToString();
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, string usings, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 36);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"{usings}namespace SampleTestProject.CsSamples 
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
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_ConditionalMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, string usings, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            string original = null;
            var updated = original?.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 37);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"{usings}namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            string original = null;
            var updated = original?.{codeFix};
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, string usings, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{methodUsed}.ToString();
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 36);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"{usings}namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{codeFix}.ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_FollowUpConditionalAccess_SurfacesDiagnostic(string methodUsed, string codeFix, string usings, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{methodUsed}?.ToString();
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 36);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"{usings}namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.{codeFix}?.ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_PrecedingAndFollowUpConditionalAccess_SurfacesDiagnostic(string methodUsed, string codeFix, string usings, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original?.{methodUsed}?.ToString();
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 37);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"{usings}namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original?.{codeFix}?.ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_PrecedingMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, string usings, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.Substring(0).{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 49);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"{usings}namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var updated = original.Substring(0).{codeFix}.ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }
    }
}