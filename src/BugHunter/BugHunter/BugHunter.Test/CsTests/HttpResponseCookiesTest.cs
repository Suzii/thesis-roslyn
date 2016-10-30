using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Kentico.Google.Apis.Util;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpResponseCookiesTest : CodeFixVerifier<HttpResponseCookiesAnalyzer, HttpResponseCookiesCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestCase(@"new System.Web.HttpResponse(""fileName"", ""url"", ""queryString"")", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))", "CookieHelper.ResponseCookies")]
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
                Message = MessagesConstants.MESSAGE.FormatString($"r.Cookies", "CookieHelper.ResponseCookies"),
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

        [TestCase(@"new System.Web.HttpResponse(""fileName"", ""url"", ""queryString"")", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))", "CookieHelper.ResponseCookies")]
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
                Message = MessagesConstants.MESSAGE.FormatString($"{instance}.Cookies", "CookieHelper.ResponseCookies"),
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

        [TestCase(@"new System.Web.HttpResponse(""fileName"", ""url"", ""queryString"")", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))", "CookieHelper.ResponseCookies")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string instance, string codeFix)
        {
            var test = $@"
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
                Message = MessagesConstants.MESSAGE.FormatString($"r.Cookies", "CookieHelper.ResponseCookies"),
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
            var cookies = {codeFix}.Count;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}