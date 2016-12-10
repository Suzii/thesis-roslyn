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
    public class HttpSessionSessionIdTest : CodeFixVerifier<HttpSessionSessionIdAnalyzer, HttpSessionSessionIdCodeFixProvider>
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_SESSION_SESSION_ID,
                Message = string.Format(MessagesConstants.MESSAGE, $"session.SessionID", "SessionHelper.GetSessionID()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 29) }
            };
            
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_SESSION_SESSION_ID,
                Message = string.Format(MessagesConstants.MESSAGE, $"{sessionInstance}.SessionID", "SessionHelper.GetSessionID()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 29) }
            };

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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_SESSION_SESSION_ID,
                Message = string.Format(MessagesConstants.MESSAGE, $"session.SessionID", "SessionHelper.GetSessionID()"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 29) }
            };

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