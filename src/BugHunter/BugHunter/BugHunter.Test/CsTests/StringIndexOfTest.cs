using BugHunter.CsRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class StringIndexOfTest : CodeFixVerifier<StringComparisonMethodsAnalyzer>
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

        [TestCase(@"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"IndexOf(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"IndexOf('a')")]
        [TestCase(@"IndexOf('a', 0)")]
        [TestCase(@"IndexOf('a', 0, 1)")]
        [TestCase(@"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"LastIndexOf('a')")]
        [TestCase(@"LastIndexOf('a', 0)")]
        [TestCase(@"LastIndexOf('a', 0, 1)")]
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

        [TestCase(@"IndexOf(""a"")", @"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"")", @"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
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
                Id = DiagnosticIds.STRING_INDEX_OF_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
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

        [TestCase(@"IndexOf(""a"")", @"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"")", @"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
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
                Id = DiagnosticIds.STRING_INDEX_OF_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 35) }
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
            //VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"IndexOf(""a"")", @"IndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"LastIndexOf(""a"")", @"LastIndexOf(""a"", StringComparison.InvariantCultureIgnoreCase)")]
       public void InputWithIncident_PrecedingMemberAccess_SurfacesDiagnostic(string methodUsed, string codeFix)
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
                Id = DiagnosticIds.STRING_INDEX_OF_METHODS,
                Message = $"'{methodUsed}' used without specifying StringComparison.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 48) }
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
            //VerifyCSharpFix(test, expectedFix);
        }
    }
}