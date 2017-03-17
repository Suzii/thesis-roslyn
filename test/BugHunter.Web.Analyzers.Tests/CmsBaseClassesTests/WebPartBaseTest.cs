using System;
using System.Linq;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.CodeFixes;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Web.Analyzers.Tests.CmsBaseClassesTests
{
    [TestFixture]
    public class WebPartBaseTest : CodeFixVerifier<WebPartBaseAnalyzer, WebPartBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.CMSBasicReferences.Union(new[]
            {
                ReferencesHelper.CMSBaseWebUI,
                ReferencesHelper.SystemWebReference,
                ReferencesHelper.SystemWebUIReference,
            }).Union(
                ReferencesHelper.GetReferencesFor(
                    typeof(CMS.PortalEngine.Web.UI.BaseEditMenu),
                    typeof(CMS.Ecommerce.Web.UI.CMSAuthorizeNetProvider),
                    typeof(CMS.UIControls.CMSAbstractUIWebpart))
            ).ToArray();
        }

        private readonly FakeFileInfo _uiWebPartFakeFileInfo = new FakeFileInfo() { FileLocation = SolutionFolders.UI_WEB_PARTS };
        private readonly FakeFileInfo _webPartFakeFileInfo = new FakeFileInfo() { FileLocation = SolutionFolders.WEB_PARTS };
        
        private static DiagnosticResult GetDiagnosticResult(string projectPath, params string[] messageArguments)
        {
            switch (projectPath)
            {
                case SolutionFolders.UI_WEB_PARTS:
                    return new DiagnosticResult
                    {
                        Id = DiagnosticIds.UI_WEB_PART_BASE,
                        Message = $"'{messageArguments[0]}' should inherit from some abstract CMS UI WebPart.",
                        Severity = DiagnosticSeverity.Warning,
                    };
                case SolutionFolders.WEB_PARTS:
                    return new DiagnosticResult
                    {
                        Id = DiagnosticIds.WEB_PART_BASE,
                        Message = $"'{messageArguments[0]}' should inherit from some abstract CMS WebPart.",
                        Severity = DiagnosticSeverity.Warning,
                    };
                default:
                    throw new ArgumentException(nameof(projectPath));
            }
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
            VerifyCSharpDiagnostic(test, new FakeFileInfo {FileLocation = excludedPath});
        }

        [Test]
        public void OkInput_NestedClass_NoDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : CMS.PortalEngine.Web.UI.CMSAbstractWebPart
    {{

         public class NestedNoDiagnostic {{ }}

         internal enum NestedEnumNoDiagnostic {{ }}
    }}
}}";

            VerifyCSharpDiagnostic(test, new FakeFileInfo { FileLocation = SolutionFolders.WEB_PARTS });
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n\r\n", SolutionFolders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WEB_PARTS)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n\r\n", SolutionFolders.WEB_PARTS)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", SolutionFolders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", SolutionFolders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", SolutionFolders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", SolutionFolders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", SolutionFolders.WEB_PARTS)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", SolutionFolders.WEB_PARTS)]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage, string usings, string fileLocation)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, new FakeFileInfo {FileLocation = fileLocation});
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

            var expectedDiagnosticForWebPart = GetDiagnosticResult(SolutionFolders.WEB_PARTS, "SampleClass").WithLocation(5, 18, _webPartFakeFileInfo);
            var expectedDiagnosticForUiWebPart = GetDiagnosticResult(SolutionFolders.UI_WEB_PARTS, "SampleClass").WithLocation(5, 18, _uiWebPartFakeFileInfo);

            VerifyCSharpDiagnostic(test, _webPartFakeFileInfo, expectedDiagnosticForWebPart);
            VerifyCSharpDiagnostic(test, _uiWebPartFakeFileInfo, expectedDiagnosticForUiWebPart);
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n\r\n", SolutionFolders.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n\r\n", SolutionFolders.UI_WEB_PARTS)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", SolutionFolders.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", SolutionFolders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", SolutionFolders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", SolutionFolders.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", SolutionFolders.UI_WEB_PARTS)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", SolutionFolders.UI_WEB_PARTS)]
        public void InputWithIncident_ClassExtendingCMSClassButOnWrongPath_SurfacesDignostic(string oldUsage, string usings, string fileLocation)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";

            var line = string.IsNullOrEmpty(usings) ? 3 : 5;
            var fakeFileInfo = new FakeFileInfo() {FileLocation = fileLocation};
            var expectedDiagnostic = GetDiagnosticResult(fileLocation, "SampleClass").WithLocation(line, 18, fakeFileInfo);

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic);
        }

        #region  CodeFixes tests - only testing CodeFix, not analyzer part

        private static readonly object[] CodeFixesTestSource = {
            new object [] {SolutionFolders.UI_WEB_PARTS, "CMSAbstractUIWebpart", "CMS.UIControls", 0},
            new object [] {SolutionFolders.WEB_PARTS, "CMSAbstractWebPart", "CMS.PortalEngine.Web.UI", 0},
            new object [] {SolutionFolders.WEB_PARTS, "CMSAbstractEditableWebPart", "CMS.PortalEngine.Web.UI", 1},
            new object [] {SolutionFolders.WEB_PARTS, "CMSAbstractLayoutWebPart", "CMS.PortalEngine.Web.UI", 2},
            new object [] {SolutionFolders.WEB_PARTS, "CMSAbstractWizardWebPart", "CMS.PortalEngine.Web.UI", 3},
            new object [] {SolutionFolders.WEB_PARTS, "CMSCheckoutWebPart", "CMS.Ecommerce.Web.UI", 4},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassNotCMSClass_ProvidesCodefixes(string fileLocation, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.WebControls.WebParts.WebPart
    {{
    }}
}}";
            var fakeFileInfo = new FakeFileInfo { FileLocation = fileLocation };
            var expectedDiagnostic = GetDiagnosticResult(fileLocation, "SampleClass").WithLocation(3, 18, fakeFileInfo);

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
        public void InputWithNoError_ClassOnlyImplementingSomeInterface_NoDiagnostic(string fileLocation, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.IDisposable
    {{
        public void Dispose() {{ }}
    }}
}}";
            var fakeFileInfo = new FakeFileInfo { FileLocation = fileLocation };

            VerifyCSharpDiagnostic(test, fakeFileInfo);
        }

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClassAndImplementingSomeInterface_ProvidesCodefixes(string fileLocation, string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass : System.Web.UI.WebControls.WebParts.WebPart, System.IDisposable
    {{
    }}
}}";
            var fakeFileInfo = new FakeFileInfo {FileLocation = fileLocation};
            var expectedDiagnostic = GetDiagnosticResult(fileLocation, "SampleClass").WithLocation(3, 18, fakeFileInfo);

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public class SampleClass : {baseClassToExtend}, System.IDisposable
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, fakeFileInfo);
        }
        #endregion
    }
}