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
    public class UserControlBaseTest : CodeFixVerifier<UserControlBaseAnalyzer, UserControlBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference }).ToArray();
        }

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.USER_CONTROL_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from some abstract CMSControl.",
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
        [TestCase(@"", @": System.Web.UI.Control")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @"")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @": System.Web.UI.Control")]
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
        [TestCase(ProjectPaths.USER_CONTROLS)]
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

        [TestCase(nameof(CMS.Base.Web.UI.AbstractUserControl), ProjectPaths.USER_CONTROLS)]
        [TestCase("CMS.Base.Web.UI.AbstractUserControl", ProjectPaths.UI_WEB_PARTS)]
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

        [TestCase(nameof(System.Web.UI.Control))]
        [TestCase("System.Web.UI.Control")]
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

            VerifyCSharpDiagnostic(test, ProjectPaths.USER_CONTROLS, expectedDiagnostic.WithLocation(5, 26, ProjectPaths.USER_CONTROLS + "Test0.cs"));
        }

        // TODO add more possibilities
        private static readonly object[] CodeFixesTestSource = {
            new object [] {ProjectPaths.USER_CONTROLS, nameof(CMSUserControl), "CMS.UIControls", 0},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string filePath, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.Control
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