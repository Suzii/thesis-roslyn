using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [Ignore("Functionality for analyzing ConditionalAccessExpressions is not yet in place")]
    [TestFixture]
    public class PageIsCallbackConditionalTest : CodeFixVerifier<PageIsCallbackAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.CMSBasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic()
        {   
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var isPostBack = new System.Web.UI.Page()?.IsCallback;
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.PAGE_IS_CALLBACK,
                Message = string.Format(MessagesConstants.MESSAGE, $"new System.Web.UI.Page()?.IsCallback", "RequestHelper.IsCallback()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 30) }
            };
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [Test]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic()
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var page = new System.Web.UI.Page();
            var answer = page?.IsCallback.ToString();
        }}
    }}
}}";

            var page = new System.Web.UI.Page();
            var answer = page?.IsCallback.ToString();

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.PAGE_IS_CALLBACK,
                Message = string.Format(MessagesConstants.MESSAGE, $"page?.IsCallback", "RequestHelper.IsCallback()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 26) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }
    }
}