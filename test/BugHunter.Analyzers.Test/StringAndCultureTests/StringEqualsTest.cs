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
    public class StringEqualsTest : CodeFixVerifier<StringEqualsMethodAnalyzer, StringComparisonMethodsWithModifierCodeFixProvider>
    {
        static readonly object[] TestSource =
        {
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.Ordinal)", 0 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.OrdinalIgnoreCase)", 1 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.CurrentCulture)", 2 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.CurrentCultureIgnoreCase)", 3 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCulture)", 4 },
            new object[] { @"Equals(""a"")", @"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)", 5 },
        };

        protected override MetadataReference[] GetAdditionalReferences() => null;

        private DiagnosticResult GetDiagnosticResult(string methodUsed)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.StringEqualsMethod,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
            };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"Equals(""a"", sc)")]
        [TestCase(@"Equals(""a"", StringComparison.InvariantCultureIgnoreCase)")]
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
            var original = ""Original string"";
            var result1 = original.{methodUsed};
            var result2 = original.{methodUsed}.ToString();
            var result3 = original?.{methodUsed};
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

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 35);
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
        public void InputWithIncident_ConditionalMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            string original = null;
            var result = original?.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 36);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            string original = null;
            var result = original?.{codeFix};
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

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 35);
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

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 48);
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