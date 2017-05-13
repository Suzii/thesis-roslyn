using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
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
    public class HttpRequestQueryStringTest : CodeFixVerifier<HttpRequestQueryStringAnalyzer>
    {
        protected override MetadataReference[] AdditionalReferences
            => new[] { ReferencesHelper.SystemWebReference };

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
            var url = request.QueryString[""key""];
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult(@"request.QueryString", "QueryHelper.Get<Type>()").WithLocation(9, 23);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
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
            var url = {requestInstance}.QueryString[""key""];
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult($@"{requestInstance}.QueryString", "QueryHelper.Get<Type>()").WithLocation(8, 23);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
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
            var url = request.QueryString[""key""].Contains(""Ooops..."");
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult(@"request.QueryString", "QueryHelper.Get<Type>()").WithLocation(9, 23);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HttpRequestQueryString,
                Message = string.Format(MessagesConstants.Message, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}