using BugHunter.Analyzers.StringAndCultureRules.Analyzers;
using BugHunter.Analyzers.StringAndCultureRules.CodeFixes;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.StringAndCultureTests
{
    [TestFixture]
    public class StringManipulationMethodsTest : CodeFixVerifier<StringManipulationMethodsAnalyzer, StringManipultionMethodsCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences() => null;

        private DiagnosticResult GetDiagnosticResult(string methodUsed)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_MANIPULATION_METHODS,
                Message = $"'{methodUsed}' used without specifying CultureInfo.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

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
            var updated = original.{methodUsed};
        }}
    }}
}}";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("ToLower()", "ToLowerInvariant()", 0)]
        [TestCase("ToLower()", "ToLower(CultureInfo.CurrentCulture)", 1)]
        [TestCase("ToUpper()", "ToUpperInvariant()", 0)]
        [TestCase("ToUpper()", "ToUpper(CultureInfo.CurrentCulture)", 1)]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
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
            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [TestCase("ToLower()", "ToLowerInvariant()", 0)]
        [TestCase("ToLower()", "ToLower(CultureInfo.CurrentCulture)", 1)]
        [TestCase("ToUpper()", "ToUpperInvariant()", 0)]
        [TestCase("ToUpper()", "ToUpper(CultureInfo.CurrentCulture)", 1)]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
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

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
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

        [TestCase("ToLower()", "ToLowerInvariant()", 0)]
        [TestCase("ToLower()", "ToLower(CultureInfo.CurrentCulture)", 1)]
        [TestCase("ToUpper()", "ToUpperInvariant()", 0)]
        [TestCase("ToUpper()", "ToUpper(CultureInfo.CurrentCulture)", 1)]
        public void InputWithIncident_PrecedingMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
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

            var expectedFix = $@"namespace SampleTestProject.CsSamples 
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