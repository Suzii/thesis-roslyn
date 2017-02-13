using BugHunter.CmsApiReplacementRules.Analyzers;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpRequestQueryStringTest : CodeFixVerifier<HttpRequestQueryStringAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return new[] {ReferencesHelper.SystemWebReference};
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_QUERY_STRING,
                Message = string.Format(MessagesConstants.MESSAGE, @"request.QueryString", "QueryHelper.Get<Type>()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 23) }
            };

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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_QUERY_STRING,
                Message = string.Format(MessagesConstants.MESSAGE, $@"{requestInstance}.QueryString", "QueryHelper.Get<Type>()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 23) }
            };

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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_REQUEST_QUERY_STRING,
                Message = string.Format(MessagesConstants.MESSAGE, @"request.QueryString", "QueryHelper.Get<Type>()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 23) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }
    }
}