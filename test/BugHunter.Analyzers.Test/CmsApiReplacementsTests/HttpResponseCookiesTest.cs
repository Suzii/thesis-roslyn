using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpResponseCookiesTest : CodeFixVerifier<HttpResponseCookiesAnalyzer, HttpResponseCookiesCodeFixProvider>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

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
            var expectedDiagnostic = CreateDiagnosticResult("r.Cookies", "CookieHelper.ResponseCookies").WithLocation(9, 27);

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
            var expectedDiagnostic = CreateDiagnosticResult($"{instance}.Cookies", "CookieHelper.ResponseCookies").WithLocation(8, 27);

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
            var expectedDiagnostic = CreateDiagnosticResult("r.Cookies", "CookieHelper.ResponseCookies").WithLocation(10, 27);

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

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HttpResponseCookies,
                Message = string.Format(MessagesConstants.Message, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}