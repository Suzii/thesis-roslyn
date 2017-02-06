using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.BaseClassesRules.CodeFixes;
using BugHunter.Test.Verifiers;
using CMS.UIControls;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.BaseClassesChecks
{
    [TestFixture]
    public class PageBaseTest : CodeFixVerifier<PageBaseAnalyzer, PageBaseCodeFixProvider>
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

        private static readonly object[] CodeFixesTestSource = {
            new object [] {ProjectPaths.PAGES, nameof(AbstractCMSPage), "CMS.UIControls", 0},
            new object [] {ProjectPaths.PAGES, nameof(CMSUIPage), "CMS.UIControls", 1},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string filePath, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.Page
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, filePath, expectedDiagnostic.WithLocation(3, 26, filePath + "Test0.cs"));

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, filePath);
        }
    }
}