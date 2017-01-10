using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class PageIsPostBackTest : CodeFixVerifier<PageIsPostBackAnalyzer, PageIsPostBackCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();
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
            var isPostBack = new System.Web.UI.Page().IsPostBack;
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.PAGE_IS_POST_BACK,
                Message = string.Format(MessagesConstants.MESSAGE, $"new System.Web.UI.Page().IsPostBack", "RequestHelper.IsPostBack()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 30) }
            };
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var isPostBack = RequestHelper.IsPostBack();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
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
            var answer = page.IsPostBack.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.PAGE_IS_POST_BACK,
                Message = string.Format(MessagesConstants.MESSAGE, "page.IsPostBack", "RequestHelper.IsPostBack()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 26) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var page = new System.Web.UI.Page();
            var answer = RequestHelper.IsPostBack().ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}