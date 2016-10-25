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
    public class HttpRequestUserHostAddressTest : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new HttpRequestUserHostAddressCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new HttpRequestUserHostAddressAnalyzer();
        }

        protected override MetadataReference[] GetAdditionalReferences()
        {
            return Constants.BasicReferences.Union(new[] {Constants.SystemWebReference}).ToArray();
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
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var address = request.UserHostAddress;
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1002",
                Message = "Property Request.UserHostAddress is being accessed.",
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
            var address = RequestContext.UserHostAddress;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}