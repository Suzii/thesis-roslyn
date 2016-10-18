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
    public class RequestUserHostAddressTest : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RequestUserHostAddressCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new RequestUserHostAddressAnalyzer();
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
        
        [Test]
        public void InputWithIncident_SurfacesDiagnostic()
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class RequestUserHostAddressAnalyzer
    {{
        public void SampleMethod()
        {{
            var request = new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"");
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
            var request = new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"");
            var address = RequestContext.UserHostAddress;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}