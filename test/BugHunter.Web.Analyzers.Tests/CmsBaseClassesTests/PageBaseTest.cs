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
    public class PageBaseTest : CodeFixVerifier<PageBaseAnalyzer_V1_SyntaxTree, PageBaseCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.CMSBasicReferences
                .Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference, ReferencesHelper.CMSUiControls })
                .ToArray();
        }

        private readonly FakeFileInfo _pagesFakeFileInfo = new FakeFileInfo {FileExtension= "aspx.cs"};

        private DiagnosticResult GetDiagnosticResult(params string[] messageArguments)
        {
            return new DiagnosticResult
            {
                Id = BugHunter.Web.Analyzers.DiagnosticIds.PAGE_BASE,
                Message = $"'{messageArguments[0]}' should inherit from some abstract CMSPage.",
                Severity = DiagnosticSeverity.Warning,
            };
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"cs", @"")]
        [TestCase(@"cs", @": System.Web.UI.Page")]
        [TestCase(@"this.should.prevent.from.diagnostic.being.raised.cs", @"")]
        [TestCase(@"this.should.prevent.from.diagnostic.being.raised.cs", @": System.Web.UI.Page")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedFileExtension, string baseList)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass {baseList}
    {{
    }}
}}";
            var fakeFileInfo = new FakeFileInfo {FileExtension = excludedFileExtension};
            VerifyCSharpDiagnostic(test, fakeFileInfo);
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

            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo);
        }

        [TestCase(nameof(CMS.UIControls.AbstractCMSPage))]
        [TestCase("CMS.UIControls.AbstractCMSPage")]
        public void OkayInput_ClassExtendingCMSClass_NoDiagnostic(string oldUsage)
        {
            var test = $@"using CMS.UIControls;

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass: {oldUsage}
    {{
    }}
}}";
            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo);
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
        public string GetName() 
        {{
            var name = nameof(SampleClass);
            return name;
        }}
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(5, 26, _pagesFakeFileInfo);

            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo, expectedDiagnostic);
        }

        private static readonly object[] CodeFixesTestSource = {
            new object [] {nameof(CMS.UIControls.AbstractCMSPage), "CMS.UIControls", 0},
            new object [] {nameof(CMS.UIControls.CMSUIPage), "CMS.UIControls", 1},
        };

        [Test, TestCaseSource(nameof(CodeFixesTestSource))]
        public void InputWithError_ClassExtendingWrongClass_ProvidesCodefixes(string baseClassToExtend, string namespaceToBeUsed, int codeFixNumber)
        {
            var test = $@"namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : System.Web.UI.Page
    {{
    }}
}}";
            var expectedDiagnostic = GetDiagnosticResult("SampleClass").WithLocation(3, 26, _pagesFakeFileInfo);

            VerifyCSharpDiagnostic(test, _pagesFakeFileInfo, expectedDiagnostic);

            var expectedFix = $@"using {namespaceToBeUsed};

namespace SampleTestProject.CsSamples
{{
    public partial class SampleClass : {baseClassToExtend}
    {{
    }}
}}";

            VerifyCSharpFix(test, expectedFix, codeFixNumber, false, _pagesFakeFileInfo);
        }
    }
}