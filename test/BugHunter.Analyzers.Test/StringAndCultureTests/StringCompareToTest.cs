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
    public class StringCompareToTest : CodeFixVerifier<StringCompareToMethodAnalyzer, StringCompareToMethodCodeFixProvider>
    {
        static readonly object[] TestSource =
        {
            new object[] { @"CompareTo(""a"")", @"string.Compare(""aa"", ""a"", StringComparison.Ordinal)", 0 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(""aa"", ""a"", StringComparison.OrdinalIgnoreCase)", 1 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(""aa"", ""a"", StringComparison.CurrentCulture)", 2 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(""aa"", ""a"", StringComparison.CurrentCultureIgnoreCase)", 3 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(""aa"", ""a"", StringComparison.InvariantCulture)", 4 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(""aa"", ""a"", StringComparison.InvariantCultureIgnoreCase)", 5 },
        };

        static readonly object[] TestSourceWithVariable =
{
            new object[] { @"CompareTo(""a"")", @"string.Compare(ouch, ""a"", StringComparison.Ordinal)", 0 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(ouch, ""a"", StringComparison.OrdinalIgnoreCase)", 1 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(ouch, ""a"", StringComparison.CurrentCulture)", 2 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(ouch, ""a"", StringComparison.CurrentCultureIgnoreCase)", 3 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(ouch, ""a"", StringComparison.InvariantCulture)", 4 },
            new object[] { @"CompareTo(""a"")", @"string.Compare(ouch, ""a"", StringComparison.InvariantCultureIgnoreCase)", 5 },
        };

        protected override MetadataReference[] GetAdditionalReferences() => null;

        private DiagnosticResult GetDiagnosticResult(string methodUsed)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.STRING_COMPARE_TO_METHOD,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Warning,
            };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

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
            var result = ""aa"".{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(7, 31);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = {codeFix};
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSourceWithVariable))]
        public void InputWithIncident_ConditionalMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            string ouch = null;
            var result = ouch.{methodUsed};
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(8, 31);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            string ouch = null;
            var result = {codeFix};
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
            var result = ""aa"".{methodUsed}.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(7, 31);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = {codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [Test, TestCaseSource(nameof(TestSource))]
        public void InputWithIncident_FollowUpConditionalAccess_SurfacesDiagnostic(string methodUsed, string codeFix, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = ""aa"".{methodUsed}.ToString()?.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = GetDiagnosticResult(methodUsed).WithLocation(7, 31);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var result = {codeFix}.ToString()?.ToString();
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
            var temp = {codeFix};
            var result = original.Substring(0).temp.ToString();
        }}
    }}
}}";
            // TODO no codfix to be applied (so far)
            VerifyCSharpFix(test, test);
        }
    }
}