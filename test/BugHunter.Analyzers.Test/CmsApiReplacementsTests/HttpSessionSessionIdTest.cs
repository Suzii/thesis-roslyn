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
    public class HttpSessionSessionIdTest : CodeFixVerifier<HttpSessionSessionIdAnalyzer, HttpSessionSessionIdCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_SESSION_SESSION_ID,
                Message = string.Format(MessagesConstants.MESSAGE, messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_SimpleMemberAccess_SurfacesDiagnostic(string sessionInstance)
        {   
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var sessionId = session.SessionID;
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult("session.SessionID", "SessionHelper.GetSessionID()").WithLocation(9, 29);
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var sessionId = SessionHelper.GetSessionID();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
        
        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_ChainedMemberAccess_SurfacesDiagnostic(string sessionInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var sessionId = {sessionInstance}.SessionID;
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult($"{sessionInstance}.SessionID", "SessionHelper.GetSessionID()").WithLocation(8, 29);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var sessionId = SessionHelper.GetSessionID();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_FollowUpMemberAccess_SurfacesDiagnostic(string sessionInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var sessionId = session.SessionID.Contains(""Ooops..."");
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult("session.SessionID", "SessionHelper.GetSessionID()").WithLocation(9, 29);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var sessionId = SessionHelper.GetSessionID().Contains(""Ooops..."");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}