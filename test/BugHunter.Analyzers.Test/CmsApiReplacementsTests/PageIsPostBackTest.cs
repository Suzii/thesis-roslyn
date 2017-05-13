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
    public class PageIsPostBackTest : CodeFixVerifier<PageIsPostBackAnalyzer, PageIsPostBackCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.PageIsPostBack,
                Message = string.Format(MessagesConstants.Message, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };

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

            var expectedDiagnostic = CreateDiagnosticResult("new System.Web.UI.Page().IsPostBack", "RequestHelper.IsPostBack()").WithLocation(8, 30);

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

            var expectedDiagnostic = CreateDiagnosticResult("page.IsPostBack", "RequestHelper.IsPostBack()").WithLocation(9, 26);

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