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
    public class FormsAuthenticationSignOutTest : CodeFixVerifier<FormsAuthenticationSignOut, FormsAuthenticationSignOutCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences.Union(ReferencesHelper.GetReferencesFor(typeof(System.Web.HtmlString), typeof(CMS.Membership.AuthenticationHelper))).ToArray();

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.FORMS_AUTHENTICATION_SIGN_OUT,
                Message = string.Format(MessagesConstants.MESSAGE, messageArgs),
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
            var test = $@"using System.Web.Security;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            FormsAuthentication.SignOut();
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult("FormsAuthentication.SignOut()", "AuthenticationHelper.SignOut()").WithLocation(9, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using System.Web.Security;
using CMS.Membership;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            AuthenticationHelper.SignOut();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix, 0, true);
        }

        [Test]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic()
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            System.Web.Security.FormsAuthentication.SignOut();
        }}
    }}
}}";
            var expectedDiagnostic = CreateDiagnosticResult("System.Web.Security.FormsAuthentication.SignOut()", "AuthenticationHelper.SignOut()").WithLocation(8, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Membership;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            AuthenticationHelper.SignOut();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}