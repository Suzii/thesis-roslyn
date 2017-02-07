using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.BaseClassesRules.CodeFixes;
using BugHunter.Test.Verifiers;
using CMS.Base.Web.UI;
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

        private readonly FakeFileInfo _userControlFakeFileInfo = new FakeFileInfo() { FileLoaction = ProjectPaths.USER_CONTROLS };
        
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
        [TestCase(@"", @": System.Web.UI.UserControl")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @"")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised", @": System.Web.UI.UserControl")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedPath, string baseList)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass {baseList}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, new FakeFileInfo() {FileLoaction = excludedPath});
        }

        [Test]
        public void InputWithError_ClassNotExtendingAnyClass_NoDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass
    {{
    }}
}}";

            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo);
        }

        [TestCase(nameof(CMS.Base.Web.UI.AbstractUserControl))]
        [TestCase("CMS.Base.Web.UI.AbstractUserControl")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo);
        }

        [TestCase(nameof(System.Web.UI.UserControl))]
        [TestCase("System.Web.UI.UserControl")]
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

            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo, expectedDiagnostic.WithLocation(5, 26, ProjectPaths.USER_CONTROLS + "Test0.cs"));
        }

        private static readonly object[] CodeFixesTestSource = {
            new object [] {nameof(CMSUserControl), "CMS.UIControls", 0},
            new object [] {nameof(AbstractUserControl), "CMS.Base.Web.UI", 1},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.UserControl
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, _userControlFakeFileInfo, expectedDiagnostic.WithLocation(3, 26, ProjectPaths.USER_CONTROLS + "Test0.cs"));

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, _userControlFakeFileInfo);
        }
    }
}