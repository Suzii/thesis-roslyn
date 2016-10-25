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
    public class HttpSessionElementAccessTest : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new HttpSessionElementAccessCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new HttpSessionElementAccessAnalyzer();
        }

        protected override MetadataReference[] GetAdditionalReferences()
        {
            return Constants.BasicReferences.Union(new[] {Constants.SystemWebReference}).ToArray();
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_GetMethod_SurfacesDiagnostic(string sessionInstance)
        {   
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class FakeController
    {{
        public void FakeIndex()
        {{
            var session = {sessionInstance};
            var aValue = session[""aKey""];
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1003",
                Message = @"""session[""aKey""]"" should not be used. Use ""SessionHelper.GetValue()"" or ""SessionHelper.SetValue()""  instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 26) }
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
            var aValue = SessionHelper.GetValue(""aKey"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }


        [TestCase("System.Web.HttpContext.Current.Session")]
        [TestCase("new System.Web.HttpSessionStateWrapper(System.Web.HttpContext.Current.Session)")]
        public void InputWithIncident_SetMethod_SurfacesDiagnostic(string sessionInstance)
        {
            var test = $@"
namespace SampleTestProject.CsSamples 
{{
    public class FakeController
    {{
        public void FakeIndex()
        {{
            var session = {sessionInstance};
            session[""aKey""] = ""aValue"";
        }}
    }}
}}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1003",
                Message = @"""session[""aKey""]"" should not be used. Use ""SessionHelper.GetValue()"" or ""SessionHelper.SetValue()""  instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
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
            SessionHelper.SetValue(""aKey"", ""aValue"");
        }}
    }}
}}";
            VerifyCSharpFix(test, expectedFix);
        }
    }
}