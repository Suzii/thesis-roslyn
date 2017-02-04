using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.BaseClassesChecks
{
    [TestFixture]
    public class PageBaseTest : CodeFixVerifier<PageBaseAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference }).ToArray();
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.PAGE_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from some abstract CMSPage.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"", @"")]
        [TestCase(@"", @": System.Web.UI.Page")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @"")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @": System.Web.UI.Page")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedPath, string baseList)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass {baseList}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, excludedPath);
        }

        // TODO is this okay? Not extending any class in defined project path without diagnostic???
        [TestCase(ProjectPaths.PAGES)]
        public void InputWithError_ClassNotExtendingAnyClass_NoDiagnostic(string filePath)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass
    {{
    }}
}}";

            VerifyCSharpDiagnostic(test, filePath);
        }

        [TestCase(nameof(System.Web.UI.Page))]
        [TestCase("System.Web.UI.Page")]
        public void InputWithError_ClassNotExtendingCMSClass_SurfacesDiagnostic(string oldUsage)
        {
            var test = $@"using System.Web.UI;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, ProjectPaths.PAGES, expectedDiagnostic.WithLocation(5, 26, ProjectPaths.PAGES + "Test0.cs"));
        }

        [TestCase(nameof(CMS.UIControls.AbstractCMSPage), ProjectPaths.PAGES)]
        [TestCase("CMS.UIControls.AbstractCMSPage", ProjectPaths.PAGES)]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage, string filePath)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, filePath);
        }
    }
}