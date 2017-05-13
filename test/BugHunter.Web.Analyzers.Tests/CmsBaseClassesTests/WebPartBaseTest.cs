using System;
using System.Linq;
using BugHunter.Core.Constants;
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
                    typeof(CMS.UIControls.CMSAbstractUIWebpart))).ToArray();
        }

        private readonly FakeFileInfo _uiWebPartFakeFileInfo = new FakeFileInfo() { FileLocation = SolutionFolders.UIWebParts };
        private readonly FakeFileInfo _webPartFakeFileInfo = new FakeFileInfo() { FileLocation = SolutionFolders.WebParts };

        private static DiagnosticResult GetDiagnosticResult(string projectPath, params string[] messageArguments)
        {
            switch (projectPath)
            {
                case SolutionFolders.UIWebParts:
                    return new DiagnosticResult
                    {
                        Id = DiagnosticIds.UIWebPartBase,
                        Message = $"'{messageArguments[0]}' should inherit from some abstract CMS UI WebPart.",
                        Severity = DiagnosticSeverity.Warning,
                    };
                case SolutionFolders.WebParts:
                    return new DiagnosticResult
                    {
                        Id = DiagnosticIds.WebPartBase,
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
            VerifyCSharpDiagnostic(test, new FakeFileInfo { FileLocation = excludedPath });
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

            VerifyCSharpDiagnostic(test, new FakeFileInfo { FileLocation = SolutionFolders.WebParts });
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n\r\n", SolutionFolders.UIWebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.WebParts)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n\r\n", SolutionFolders.WebParts)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", SolutionFolders.UIWebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", SolutionFolders.WebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", SolutionFolders.WebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", SolutionFolders.WebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", SolutionFolders.WebParts)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", SolutionFolders.WebParts)]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage, string usings, string fileLocation)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, new FakeFileInfo { FileLocation = fileLocation });
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

            var expectedDiagnosticForWebPart = GetDiagnosticResult(SolutionFolders.WebParts, "SampleClass").WithLocation(5, 18, _webPartFakeFileInfo);
            var expectedDiagnosticForUiWebPart = GetDiagnosticResult(SolutionFolders.UIWebParts, "SampleClass").WithLocation(5, 18, _uiWebPartFakeFileInfo);

            VerifyCSharpDiagnostic(test, _webPartFakeFileInfo, expectedDiagnosticForWebPart);
            VerifyCSharpDiagnostic(test, _uiWebPartFakeFileInfo, expectedDiagnosticForUiWebPart);
        }

        [TestCase(nameof(CMS.UIControls.CMSAbstractUIWebpart), "using CMS.UIControls;\r\n\r\n", SolutionFolders.WebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UIWebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UIWebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UIWebParts)]
        [TestCase(nameof(CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart), "using CMS.PortalEngine.Web.UI;\r\n\r\n", SolutionFolders.UIWebParts)]
        [TestCase(nameof(CMS.Ecommerce.Web.UI.CMSCheckoutWebPart), "using CMS.Ecommerce.Web.UI;\r\n\r\n", SolutionFolders.UIWebParts)]
        [TestCase("CMS.UIControls.CMSAbstractUIWebpart", "", SolutionFolders.WebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWebPart", "", SolutionFolders.UIWebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractEditableWebPart", "", SolutionFolders.UIWebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractLayoutWebPart", "", SolutionFolders.UIWebParts)]
        [TestCase("CMS.PortalEngine.Web.UI.CMSAbstractWizardWebPart", "", SolutionFolders.UIWebParts)]
        [TestCase("CMS.Ecommerce.Web.UI.CMSCheckoutWebPart", "", SolutionFolders.UIWebParts)]
        public void InputWithIncident_ClassExtendingCMSClassButOnWrongPath_SurfacesDignostic(string oldUsage, string usings, string fileLocation)
        {
            var test = $@"{usings}namespace SampleTestProject.CsSamples
{{
    public class SampleClass: {oldUsage}
    {{
    }}
}}";

            var line = string.IsNullOrEmpty(usings) ? 3 : 5;
            var fakeFileInfo = new FakeFileInfo() { FileLocation = fileLocation };
            var expectedDiagnostic = GetDiagnosticResult(fileLocation, "SampleClass").WithLocation(line, 18, fakeFileInfo);

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic);
        }

        private static readonly object[] CodeFixesTestSource =
        {
            new object[] { SolutionFolders.UIWebParts, "CMSAbstractUIWebpart", "CMS.UIControls", 0 },
            new object[] { SolutionFolders.WebParts, "CMSAbstractWebPart", "CMS.PortalEngine.Web.UI", 0 },
            new object[] { SolutionFolders.WebParts, "CMSAbstractEditableWebPart", "CMS.PortalEngine.Web.UI", 1 },
            new object[] { SolutionFolders.WebParts, "CMSAbstractLayoutWebPart", "CMS.PortalEngine.Web.UI", 2 },
            new object[] { SolutionFolders.WebParts, "CMSAbstractWizardWebPart", "CMS.PortalEngine.Web.UI", 3 },
            new object[] { SolutionFolders.WebParts, "CMSCheckoutWebPart", "CMS.Ecommerce.Web.UI", 4 },
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
            var fakeFileInfo = new FakeFileInfo { FileLocation = fileLocation };
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
    }
}