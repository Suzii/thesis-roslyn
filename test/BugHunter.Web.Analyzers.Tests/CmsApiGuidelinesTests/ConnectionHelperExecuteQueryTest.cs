using System.Linq;
using BugHunter.Core.Constants;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using BugHunter.Web.Analyzers.CmsApiGuidelinesRules.Analyzers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Web.Analyzers.Tests.CmsApiGuidelinesTests
{
    [TestFixture]
    public class ConnectionHelperExecuteQueryTest : CodeFixVerifier<ConnectionHelperExecuteQueryAnalyzer>
    {
        protected override MetadataReference[] AdditionalReferences
            => ReferencesHelper.CMSBasicReferences.Union(ReferencesHelper.GetReferencesFor(typeof(System.Data.DataSet))).ToArray();

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase(@"")]
        [TestCase(@"this\should\prevent\from\diagnostic\being\raised")]
        public void OkInput_ClassOnExcludedPath_NoDiagnostic(string excludedPath)
        {
            var test = @"using System.Data;

namespace SampleTestProject.CsSamples
{
    public partial class SampleClass
    {
        public void SampleMethod()
        {
            CMS.DataEngine.ConnectionHelper.ExecuteQuery(null);
        }
    }
}";
            var fakeFileInfo = new FakeFileInfo { FileLocation = excludedPath };
            VerifyCSharpDiagnostic(test, fakeFileInfo);
        }

        [TestCase("aspx.cs")]
        [TestCase("ascx.cs")]
        [TestCase("ashx.cs")]
        [TestCase("master.cs")]
        public void InputWithError_ExecuteQueryCalledFromUi_SurfacesDiagnostic(string fileExtension)
        {
            var test = @"using System.Data;

using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public partial class SampleClass
    {
        public void SampleMethod() {
            ConnectionHelper.ExecuteQuery(null);
            CMS.DataEngine.ConnectionHelper.ExecuteQuery(null);
        }
    }
}";
            var fakeFileInfo = new FakeFileInfo { FileExtension = fileExtension };
            var expectedDiagnostic1 = GetDiagnosticResult("ConnectionHelper.ExecuteQuery(null)").WithLocation(10, 13, fakeFileInfo);
            var expectedDiagnostic2 = GetDiagnosticResult("CMS.DataEngine.ConnectionHelper.ExecuteQuery(null)").WithLocation(11, 13, fakeFileInfo);

            VerifyCSharpDiagnostic(test, fakeFileInfo, expectedDiagnostic1, expectedDiagnostic2);
        }

        private DiagnosticResult GetDiagnosticResult(string usage)
            => new DiagnosticResult
            {
                Id = DiagnosticIds.ConnectionHelperExecuteQuery,
                Message = $"'{usage}' should not be called directly from this file. Move the logic to codebehind instead.",
                Severity = DiagnosticSeverity.Warning,
            };
    }
}