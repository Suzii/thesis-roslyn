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
    public class FormsAuthenticationSignOutTest : CodeFixVerifier<FormsAuthenticationSignOut, FormsAuthenticationSignOutCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.SystemWebReference, ReferencesHelper.CMSMembershipReference }).ToArray();
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
            var formsAuthentication = System.Web.Security.FormsAuthentication;
            formsAuthentication.SignOut();
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.FORMS_AUTHENTICATION_SIGN_OUT,
                Message = MessagesConstants.MESSAGE.FormatString("formsAuthentication.SignOut", "AuthenticationHelper.SignOut()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Membership;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var formsAuthentication = System.Web.Security.FormsAuthentication;
            AuthenticationHelper.SignOut();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.FORMS_AUTHENTICATION_SIGN_OUT,
                Message = MessagesConstants.MESSAGE.FormatString("System.Web.Security.FormsAuthentication.SignOut", "AuthenticationHelper.SignOut()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 13) }
            };

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