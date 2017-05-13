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
    public class HttpRequestUrlTest : CodeFixVerifier<HttpRequestUrlAnalyzer, HttpRequestUrlCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HttpRequestUrl,
                Message = string.Format(MessagesConstants.Message, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var url = request.Url;
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult("request.Url", "RequestContext.URL").WithLocation(9, 23);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var url = RequestContext.URL;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var url = {requestInstance}.Url;
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($"{requestInstance}.Url", "RequestContext.URL").WithLocation(8, 23);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var url = RequestContext.URL;
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase(@"new System.Web.HttpRequest(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpRequestWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string requestInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var url = request.Url.Host.Contains(""Ooops..."");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult("request.Url", "RequestContext.URL").WithLocation(9, 23);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var request = {requestInstance};
            var url = RequestContext.URL.Host.Contains(""Ooops..."");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}