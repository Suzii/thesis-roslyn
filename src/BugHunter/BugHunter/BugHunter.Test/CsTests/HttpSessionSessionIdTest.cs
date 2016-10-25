using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
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
        public void InputWithIncident_SurfacesDiagnostic(string sessionInstance)
        {   
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class FakeController
    {{
        public void FakeIndex()
        {{
            var session = {sessionInstance};
            var sessionId = session.SessionID;
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1004",
                Message = @"""session.SessionID"" should not be used. Use ""SessionHelper.GetSessionID()"" instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 29) }
            };
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class FakeController
    {{
        public void FakeIndex()
        {{
            var session = {sessionInstance};
            var sessionId = SessionHelper.GetSessionID();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}