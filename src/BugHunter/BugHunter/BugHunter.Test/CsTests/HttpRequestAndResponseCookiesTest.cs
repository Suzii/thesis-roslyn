using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpRequestAndResponseCookiesTest : CodeFixVerifier<HttpRequestAndResponseCookiesAnalyzer, HttpRequestAndResponseCookiesCodeFixProvider>
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
        
        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")", "CookieHelper.RequestCookies")]
        [TestCase(@"new System.Web.HttpResponse(""fileName"", ""url"", ""queryString"")", "CookieHelper.ResponseCookies")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))", "CookieHelper.RequestCookies")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))", "CookieHelper.ResponseCookies")]
        public void InputWithIncident_SurfacesDiagnostic(string instance, string codeFix)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var r = {instance};
            var address = r.Cookies;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1005",
                Message = @"""r.Cookies"" should not be used. Use ""CookieHelper.ResponseCookies"" or ""CookieHelper.RequestCookies"" instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var r = {instance};
            var address = {codeFix};
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}