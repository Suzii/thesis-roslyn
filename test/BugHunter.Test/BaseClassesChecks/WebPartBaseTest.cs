using System;
using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.BaseClassesRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.BaseClassesChecks
{
    [TestFixture]
    public class WebPartBaseTest : CodeFixVerifier<WebPartBaseAnalyzer, WebPartBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[]
            {
                ReferencesHelper.CMSBaseWebUI,
                ReferencesHelper.SystemWebReference,
                ReferencesHelper.SystemWebUIReference,
                ReferencesHelper.CMSPortalEngineWebUI,
                ReferencesHelper.CMSEcommerceWebUI,
                ReferencesHelper.CMSUIControls
            }).ToArray();
        }

        private readonly FakeFileInfo _uiWebPartFakeFileInfo = new FakeFileInfo() { FileLoaction = FilePaths.Folders.UI_WEB_PARTS };
        private readonly FakeFileInfo _webPartFakeFileInfo = new FakeFileInfo() { FileLoaction = FilePaths.Folders.WEB_PARTS };
        
        private static DiagnosticResult GetDiagnosticResult(string projectPath, params string[] messageArgumentStrings)
        {
            var webPartDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.WEB_PART_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from CMS<something>WebPart.",
                Severity = DiagnosticSeverity.Warning,
            };

            var uiWebPartDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.UI_WEB_PART_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from CMS<something>WebPart.",
                Severity = DiagnosticSeverity.Warning,
            };

            return projectPath == FilePaths.Folders.UI_WEB_PARTS ? uiWebPartDiagnostic : webPartDiagnostic;
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        public void OkInput_NonPublicClass_NoDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    internal class SampleClass
    {{
    }}

    private class SampleClass2
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"")]
        [TestCase(@"_files\")]
        [TestCase(@"\this\should\prevent\from\diagnostic\being\raised")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedPath)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, new FakeFileInfo {FileLoaction = excludedPath});
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n\r\n", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.WEB_PARTS)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n\r\n", FilePaths.Folders.WEB_PARTS)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", FilePaths.Folders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", FilePaths.Folders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", FilePaths.Folders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", FilePaths.Folders.WEB_PARTS)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", FilePaths.Folders.WEB_PARTS)]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage, string usings, string filePath)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, new FakeFileInfo {FileLoaction = filePath});
        }

        [Test]
        public void InputWithError_ClassNotExtendingAnyClass_NoDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, _webPartFakeFileInfo);
            VerifyCSharpDiagnostic(test, _uiWebPartFakeFileInfo);
        }

        [Test]
        public void InputWithError_ClassNotExtendingCMSClass_SurfacesDiagnostic()
        {
            var test = $@"using System.Web.UI.WebControls.WebParts;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.WebControls.WebParts.WebPart
    {{
    }}
}}";

            var expectedDiagnosticForWebPart = GetDiagnosticResult(FilePaths.Folders.WEB_PARTS, "SampleClass").WithLocation(3, 18, FilePaths.Folders.WEB_PARTS + "Test0.cs");
            var expectedDiagnosticForUiWebPart = GetDiagnosticResult(FilePaths.Folders.UI_WEB_PARTS, "SampleClass").WithLocation(3, 18, FilePaths.Folders.UI_WEB_PARTS + "Test0.cs");

            VerifyCSharpDiagnostic(test, _webPartFakeFileInfo, expectedDiagnosticForWebPart.WithLocation(5, 18, FilePaths.Folders.WEB_PARTS + "Test0.cs"));
            VerifyCSharpDiagnostic(test, _uiWebPartFakeFileInfo, expectedDiagnosticForUiWebPart.WithLocation(5, 18, FilePaths.Folders.UI_WEB_PARTS + "Test0.cs"));
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n\r\n", FilePaths.Folders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n\r\n", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", FilePaths.Folders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", FilePaths.Folders.UI_WEB_PARTS)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", FilePaths.Folders.UI_WEB_PARTS)]
        public void InputWithIncident_ClassExtendingCMSClassButOnWrongPath_SurfacesDignostic(string oldUsage, string usings, string filePath)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";

            var line = string.IsNullOrEmpty(usings) ? 3 : 5;
            var expectedDiagnostic = GetDiagnosticResult(filePath, "SampleClass").WithLocation(line, 18, filePath + "Test0.cs");;

            VerifyCSharpDiagnostic(test, new FakeFileInfo {FileLoaction = filePath}, expectedDiagnostic);
        }

        #region  CodeFixes tests - only testing CodeFix, not analyzer part

        private static readonly object[] CodeFixesTestSource = {
            new object [] {FilePaths.Folders.UI_WEB_PARTS, "CMSAbstractUIWebpart", "CMS.UIControls", 0},
            new object [] {FilePaths.Folders.WEB_PARTS, "CMSAbstractWebPart", "CMS.PortalEngine.Web.UI", 0},
            new object [] {FilePaths.Folders.WEB_PARTS, "CMSAbstractEditableWebPart", "CMS.PortalEngine.Web.UI", 1},
            new object [] {FilePaths.Folders.WEB_PARTS, "CMSAbstractLayoutWebPart", "CMS.PortalEngine.Web.UI", 2},
            new object [] {FilePaths.Folders.WEB_PARTS, "CMSAbstractWizardWebPart", "CMS.PortalEngine.Web.UI", 3},
            new object [] {FilePaths.Folders.WEB_PARTS, "CMSCheckoutWebPart", "CMS.Ecommerce.Web.UI", 4},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassNotCMSClass_ProvidesCodefixes(string filePath, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var fakeFileInfo = new FakeFileInfo {FileLoaction = filePath};
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.WebControls.WebParts.Part
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(filePath, "SampleClass").WithLocation(3, 18, filePath + "Test0.cs");

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, fakeFileInfo);
        }

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassImplementingSomeInterface_ProvidesCodefixes(string filePath, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var fakeFileInfo = new FakeFileInfo {FileLoaction = filePath};
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.IDisposable
    {{
        public override void Dispose() {{ }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(filePath, "SampleClass").WithLocation(3, 18, filePath + "Test0.cs");
            
            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : {baseClassToExtend}, System.IDisposable
    {{
        public override void Dispose() {{ }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, fakeFileInfo);
        }

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClassAndImplementingSomeInterface_ProvidesCodefixes(string filePath, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var fakeFileInfo = new FakeFileInfo {FileLoaction = filePath};
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.WebControls.WebParts.Part, System.IDisposable
    {{
        public override void Dispose() {{ }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult(filePath, "SampleClass").WithLocation(3, 18, filePath + "Test0.cs");

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : {baseClassToExtend}, System.IDisposable
    {{
        public override void Dispose() {{ }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, fakeFileInfo);
        }
        #endregion
    }
}