using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpResponseRedirectTest : CodeFixVerifier<HttpResponseRedirectAnalyzer, HttpResponseRedirectCodeFixProvider>
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

        [TestCase(@"new System.Web.HttpResponse(null)", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponse(null)", "LocalRedirect", 1)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "LocalRedirect", 1)]
        public void InputWithIncident_SipleMemberAccess_SurfacesDiagnostic(string instance, string codeFix, int codeFixNumber)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{ 
            var r = {instance};
            r.Redirect(""url"");
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_RESPONSE_REDIRECT,
                Message = string.Format(MessagesConstants.MESSAGE_NO_SUGGESTION, $"r.Redirect"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
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
            UrlHelper.{codeFix}(""url"");
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }

        [TestCase(@"new System.Web.HttpResponse(null)", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponse(null)", "LocalRedirect", 1)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "Redirect", 0)]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpResponse(null))", "LocalRedirect", 1)]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string instance, string codeFix, int codeFixNumber)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            {instance}.Redirect(""url"");
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_RESPONSE_REDIRECT,
                Message = string.Format(MessagesConstants.MESSAGE_NO_SUGGESTION, $"{instance}.Redirect"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 13) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            UrlHelper.{codeFix}(""url"");
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber);
        }
    }
}