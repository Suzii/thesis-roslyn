using System.Linq;
using BugHunter.CmsApiReplacementRules.Analyzers;
using BugHunter.CmsApiReplacementRules.CodeFixes;
using BugHunter.Test.CmsApiReplacementsTests.Constants;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpResponseCookiesTest : CodeFixVerifier<HttpResponseCookiesAnalyzer, HttpResponseCookiesCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestCase(@"new System.Web.HttpResponse(null)", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "CookieHelper.ResponseCookies")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string instance, string codeFix)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var r = {instance};
            var cookies = r.Cookies;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_RESPONSE_COOKIES,
                Message = string.Format(MessagesConstants.MESSAGE, "r.Cookies", "CookieHelper.ResponseCookies"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var r = {instance};
            var cookies = {codeFix};
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpResponse(null)", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "CookieHelper.ResponseCookies")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string instance, string codeFix)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var cookies = {instance}.Cookies;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_RESPONSE_COOKIES,
                Message = string.Format(MessagesConstants.MESSAGE, $"{instance}.Cookies", "CookieHelper.ResponseCookies"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var cookies = {codeFix};
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpResponse(null)", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "CookieHelper.ResponseCookies")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string instance, string codeFix)
        {
            var test = $@"using System;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var r = {instance};
            var cookies = r.Cookies.Count;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_RESPONSE_COOKIES,
                Message = string.Format(MessagesConstants.MESSAGE, "r.Cookies", "CookieHelper.ResponseCookies"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System;
using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var r = {instance};
            var cookies = {codeFix}.Count;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}