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
    public class PageIsCallbackTest : CodeFixVerifier<PageIsCallbackAnalyzer, PageIsCallbackCodeFixProvider>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

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
            var isPostBack = new System.Web.UI.Page().IsCallback;
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult($"new System.Web.UI.Page().IsCallback", "RequestHelper.IsCallback()").WithLocation(8, 30);
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var isPostBack = RequestHelper.IsCallback();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithIncident_ConditionalAccess_SurfacesDiagnostic()
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

            var expectedDiagnostic = CreateDiagnosticResult($"new System.Web.UI.Page()?.IsCallback", "RequestHelper.IsCallback()").WithLocation(8, 30);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            // No fix
            VerifyCSharpFix(test, test);
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
            var answer = page.IsCallback.ToString();
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult("page.IsCallback", "RequestHelper.IsCallback()").WithLocation(9, 26);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var page = new System.Web.UI.Page();
            var answer = RequestHelper.IsCallback().ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithIncident_ConditionalAccessWithFollowUpMemberAccess_SurfacesDiagnostic()
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

            var expectedDiagnostic = CreateDiagnosticResult("page?.IsCallback", "RequestHelper.IsCallback()").WithLocation(9, 26);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            // No fix
            VerifyCSharpFix(test, test);
        }

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.PageIsCallback,
                Message = string.Format(MessagesConstants.Message, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}