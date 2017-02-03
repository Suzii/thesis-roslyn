using System.Linq;
using BugHunter.Test.Verifiers;
using BugHunter.WpRules.Analyzers;
using BugHunter.WpRules.CodeFixes;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.WpTests
{
    [TestFixture]
    public class WebPartBaseTest : CodeFixVerifier<WebPartBaseAnalyzer, WebPartBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(new[] {ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference}).ToArray();
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

        [Test]
        public void InputWithError_ClassNotExtendingAnyClass_SurfacesDiagnostic()
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [TestCase(nameof(System.Web.UI.WebControls.WebParts.WebPart))]
        [TestCase(nameof(System.Web.UI.WebControls.WebParts.Part))]
        public void InputWithError_ClassNotExtendingCMSClass_SurfacesDiagnostic(string oldUsage)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass");

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        // TODO could not find CMSAbstractLanguageWebPart, CMSAbstractWireframeWebPart, SocialMediaAbstractWebPart
        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart))] // UI webpart - must have different file location
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart))]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart))]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart))]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart))]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart))]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart")] // UI webpart - must have different file location
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart")]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart")]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart")]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart")]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test);
        }
    }
}