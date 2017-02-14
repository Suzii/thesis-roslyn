using System;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using BugHunter.Web.Analyzers.WebInternalGuidelinesRules.Analyzers;
using BugHunter.Web.Analyzers.WebInternalGuidelinesRules.CodeFixes;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Web.Analyzers.Tests.WebInternalGuidelinesTests
{
    [TestFixture]
    public class ValidationHelperGetTest : CodeFixVerifier<ValidationHelperGetAnalyzer, ValidationHelperGetCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.CMSBasicReferences;
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArguments)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.VALIDATION_HELPER_GET,
                Message = string.Format("Do not use {0}(). Use Get method with 'System' instead to ensure specific culture representation.", messageArguments),
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"GetDouble(""0"", 0)", @"GetDoubleSystem(""0"", 0)")]
        [TestCase(@"GetDouble(""0"", 0, ""en-us"")", @"GetDoubleSystem(""0"", 0)")]
        [TestCase(@"GetDouble(""0"", 0, CultureInfo.CurrentUICulture)", @"GetDoubleSystem(""0"", 0)")]
        [TestCase(@"GetDecimal(""0"", 0)", @"GetDecimalSystem(""0"", 0)")]
        [TestCase(@"GetDecimal(""0"", 0, CultureInfo.CurrentUICulture)", @"GetDecimalSystem(""0"", 0)")]
        [TestCase(@"GetDate(""0"", DateTime.MaxValue)", @"GetDateSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDate(""0"", DateTime.MaxValue, CultureInfo.CurrentUICulture)", @"GetDateSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDateTime(""0"", DateTime.MaxValue)", @"GetDateTimeSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDateTime(""0"", DateTime.MaxValue, ""en-us"")", @"GetDateTimeSystem(""0"", DateTime.MaxValue)")]
        [TestCase(@"GetDateTime(""0"", DateTime.MaxValue, CultureInfo.CurrentUICulture)", @"GetDateTimeSystem(""0"", DateTime.MaxValue)")]
        public void InputWithError_SimpleMemberAccess_SurfacesDiagnostic(string oldUsage, string codeFix)
        {
            var test = $@"using System;
using System.Globalization;
using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            ValidationHelper.{oldUsage}.ToString();
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(oldUsage.Substring(0, oldUsage.IndexOf("(", StringComparison.Ordinal))).WithLocation(11, 30);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
            var expectedFix = $@"using System;
using System.Globalization;
using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            ValidationHelper.{codeFix}.ToString();
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, null, true);
        }
    }
}