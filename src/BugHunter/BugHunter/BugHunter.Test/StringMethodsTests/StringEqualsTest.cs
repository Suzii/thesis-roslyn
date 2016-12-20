using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.StringMethodsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringEqualsTest : CodeFixVerifier<StringEqualsMethodAnalyzer, StringComparisonMethodsWithModifierCodeFixProvider>
    {
        static readonly object[] TestSource =
        {
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.CurrentCulture)", 0 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.CurrentCultureIgnoreCase)", 1 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCulture)", 2 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)", 3 },
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

        [TestCase(@"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"Equals(""a"", false, CultureInfo.CurrentCulture)")]
        public void AllowedOverloadCalled_NoDiagnostic(string methodUsed)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{methodUsed};
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
            var original = ""Original string"";
            var result = original.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_EQUALS_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{codeFix};
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
            var original = ""Original string"";
            var result = original.{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_EQUALS_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_PrecedingMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.Substring(0).{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_EQUALS_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 48) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var original = ""Original string"";
            var result = original.Substring(0).{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }
    }
}