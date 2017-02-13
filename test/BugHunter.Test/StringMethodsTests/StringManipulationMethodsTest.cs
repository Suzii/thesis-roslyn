using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.StringMethodsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringManipulationMethodsTest : CodeFixVerifier<StringManipulationMethodsAnalyzer, StringManipultionMethodsCodeFixProvider>
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

        [TestCase("ToLowerInvariant()")]
        [TestCase("ToLower(CultureInfo.CurrentCulture)")]
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_MANIPULATION_METHODS,
                Message = $"'{methodUsed}' used without specifying CultureInfo.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 36) }
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_MANIPULATION_METHODS,
                Message = $"'{methodUsed}' used without specifying CultureInfo.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 36) }
            };

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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_MANIPULATION_METHODS,
                Message = $"'{methodUsed}' used without specifying CultureInfo.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 49) }
            };

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