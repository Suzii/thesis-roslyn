using BugHunter.StringMethodsRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.StringMethodsTests
{
    [TestFixture]
    public class StringStartsEndsWithTest : CodeFixVerifier<StringStartAndEndsWithMethodsAnalyzer>
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

        [TestCase(@"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"StartsWith(""a"", false, CultureInfo.CurrentCulture)")]
        [TestCase(@"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"", false, CultureInfo.CurrentCulture)")]
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

        [TestCase(@"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
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
                Id = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS,
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

        [TestCase(@"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
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
                Id = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS,
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

        [TestCase(@"StartsWith(""a"")", @"StartsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
        [TestCase(@"EndsWith(""a"")", @"EndsWith(""a"", StringComparison.InvariantCultureIgnoreCase)")]
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
                Id = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS,
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