using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpRequestBrowserTest : CodeFixVerifier<HttpRequestBrowserAnalyzer>
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
        public void InputWithIncident_SurfacesDiagnostic(string requestInstance)
        {
//            var test = $@"
//namespace SampleTestProject.CsSamples
//{{
//    public class RequestUserHostAddressAnalyzer
//    {{
//        public void SampleMethod()
//        {{
//            var request = {requestInstance};
//            var browser = request.Browser.Browser;
//        }}
//    }}
//}}";
//            var expectedDiagnostic = new DiagnosticResult
//            {
//                Id = DiagnosticIds.HttpRequestUrl,
//                Message = @"'request.Browser' should not be used. Use 'BrowserHelper.Browser' instead.",
//                Severity = DiagnosticSeverity.Warning,
//                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 31) }
//            };

//            VerifyCSharpDiagnostic(test, expectedDiagnostic);

//            var expectedFix = $@"using CMS.Helpers;

//namespace SampleTestProject.CsSamples
//{{
//    public class RequestUserHostAddressAnalyzer
//    {{
//        public void SampleMethod()
//        {{
//            var request = {requestInstance};
//            var browser = BrowserHelper.Browser.Browser;
//        }}
//    }}
//}}";
//            VerifyCSharpFix(test, expectedFix);
        }
    }
}