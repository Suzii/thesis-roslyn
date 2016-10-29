using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpRequestBrowserTest : CodeFixVerifier<HttpRequestBrowserAnalyzer, HttpRequestBrowserCodeFixProvider>
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

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_ChainedMemeberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browser = request.Browser.Browser;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HttpRequestUrl,
                Message = @"'request.Browser.Browser' should not be used. Use 'BrowserHelper.GetBrowser()' instead.",
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
            var request = {requestInstance};
            var browser = BrowserHelper.GetBrowser();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = browserInfo.Browser;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HttpRequestUrl,
                Message = @"'browserInfo.Browser' should not be used. Use 'BrowserHelper.GetBrowser()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 10, 27) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var browserInfo = request.Browser;
            var browser = BrowserHelper.GetBrowser();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}