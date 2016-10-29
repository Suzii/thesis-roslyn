using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Kentico.Google.Apis.Util;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class HttpResponseRedirectTest : CodeFixVerifier<HttpResponseRedirectAnalyzer>
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

        [TestCase(@"new System.Web.HttpResponse(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_SipleMemberAccess_SurfacesDiagnostic(string instance)
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
                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($"r.Redirect"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestCase(@"new System.Web.HttpResponse(""fileName"", ""url"", ""queryString"")")]
        [TestCase(@"new System.Web.HttpResponseWrapper(new System.Web.HttpRequest(""fileName"", ""url"", ""queryString""))")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string instance)
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
                Message = MessagesConstants.MESSAGE_NO_SUGGESTION.FormatString($"{instance}.Redirect"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 13) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }
    }
}