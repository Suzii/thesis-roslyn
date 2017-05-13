using System.Linq;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.CodeFixes;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.CmsApiReplacementsTests
{
    [TestFixture]
    public class HttpSessionElementAccessSetTest : CodeFixVerifier<HttpSessionElementAccessAnalyzer, HttpSessionElementAccessSetCodeFixProvider>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences.Union(new[] { ReferencesHelper.SystemWebReference }).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

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
            session[""aKey""] = ""aValue"";
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult(@"session[""aKey""]").WithLocation(9, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            SessionHelper.SetValue(""aKey"", ""aValue"");
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
            {sessionInstance}[""aKey""] = ""aValue"";
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult($@"{sessionInstance}[""aKey""]").WithLocation(8, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            SessionHelper.SetValue(""aKey"", ""aValue"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_ExpressionWithPerantheses_SurfacesDiagnostic(string sessionInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            (session[""aKey""]) = ""aValue"";
        }}
    }}
}}";

            var expectedDiagnostic = CreateDiagnosticResult(@"session[""aKey""]").WithLocation(9, 13);

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.Helpers;

namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var session = {sessionInstance};
            SessionHelper.SetValue(""aKey"", ""aValue"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.HttpSessionElementAccessSet,
                Message = string.Format(@"'{0}' should not be used. Use 'SessionHelper.SetValue()' instead.", messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };
    }
}