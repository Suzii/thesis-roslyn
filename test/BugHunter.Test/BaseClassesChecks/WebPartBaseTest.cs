using System.Linq;
using BugHunter.BaseClassesRules.Analyzers;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.BaseClassesChecks
{
    [TestFixture]
    public class WebPartBaseTest : CodeFixVerifier<WebPartBaseAnalyzer>
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

        private DiagnosticResult GetDiagnosticResult(params string[] messageArgumentStrings)
        {
            return new DiagnosticResult
            {
                Id = DiagnosticIds.WEB_PART_BASE,
                Message = $"'{messageArgumentStrings[0]}' should inherit from CMS<something>WebPart.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

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
            VerifyCSharpDiagnostic(test, excludedPath);
        }

        [TestCase(ProjectPaths.UI_WEB_PARTS)]
        [TestCase(ProjectPaths.WEB_PARTS)]
        public void InputWithError_ClassNotExtendingAnyClass_SurfacesDiagnostic(string filePath)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(3, 18, filePath + "Test0.cs");

            VerifyCSharpDiagnostic(test, filePath, expectedDiagnostic);
        }

        [TestCase(nameof(System.Web.UI.WebControls.WebParts.WebPart))]
        [TestCase(nameof(System.Web.UI.WebControls.WebParts.Part))]
        public void InputWithError_ClassNotExtendingCMSClass_SurfacesDiagnostic(string oldUsage)
        {
            var test = $@"using System.Web.UI.WebControls.WebParts;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, ProjectPaths.UI_WEB_PARTS, expectedDiagnostic.WithLocation(5, 18, ProjectPaths.UI_WEB_PARTS + "Test0.cs"));
            VerifyCSharpDiagnostic(test, ProjectPaths.WEB_PARTS, expectedDiagnostic.WithLocation(5, 18, ProjectPaths.WEB_PARTS + "Test0.cs"));
        }

        // TODO could not find CMSAbstractLanguageWebPart, CMSAbstractWireframeWebPart, SocialMediaAbstractWebPart
        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n", ProjectPaths.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.WEB_PARTS)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n", ProjectPaths.WEB_PARTS)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", ProjectPaths.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", ProjectPaths.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", ProjectPaths.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", ProjectPaths.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", ProjectPaths.WEB_PARTS)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", ProjectPaths.WEB_PARTS)]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage, string usings, string filePath)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, filePath);
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n", ProjectPaths.WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n", ProjectPaths.UI_WEB_PARTS)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n", ProjectPaths.UI_WEB_PARTS)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", ProjectPaths.WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", ProjectPaths.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", ProjectPaths.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", ProjectPaths.UI_WEB_PARTS)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", ProjectPaths.UI_WEB_PARTS)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", ProjectPaths.UI_WEB_PARTS)]
        public void InputWithIncident_ClassExtendingCMSClassButOnWrongPath_SurfacesDignostic(string oldUsage, string usings, string filePath)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";

            var line = string.IsNullOrEmpty(usings) ? 3 : 4;
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(line, 18, filePath + "Test0.cs");

            VerifyCSharpDiagnostic(test, filePath, expectedDiagnostic);
        }
    }
}