using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpSessionElementAccessGetTest : CodeFixVerifier<HttpSessionElementAccessAnalyzer, HttpSessionElementAccessGetCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
            => ReferencesHelper.CMSBasicReferences.Union(new[] {ReferencesHelper.SystemWebReference}).ToArray();

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HTTP_SESSION_ELEMENT_ACCESS_GET,
                Message = string.Format(@"'{0}' should not be used. Use 'SessionHelper.GetValue()' instead.", messageArgs),
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
        public void InputWithIncident_SimpleAccess_SurfacesDiagnostic(string sessionInstance)
        {   
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var aValue = session[""aKey""];
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult(@"session[""aKey""]").WithLocation(9, 26);
            
            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var aValue = SessionHelper.GetValue(""aKey"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_ChainedAccess_SurfacesDiagnostic(string sessionInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var aValue = {sessionInstance}[""aKey""];
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult($@"{sessionInstance}[""aKey""]").WithLocation(8, 26);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var aValue = SessionHelper.GetValue(""aKey"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_FollowUpAccess_SurfacesDiagnostic(string sessionInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var aValue = session[""aKey""].ToString();
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult(@"session[""aKey""]").WithLocation(9, 26);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            var aValue = SessionHelper.GetValue(""aKey"").ToString();
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}