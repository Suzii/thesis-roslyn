using System.Linq;
using BugHunter.CsRules.Analyzers;
using BugHunter.Test.Shared;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class LucerneSearchDocumentTest : CodeFixVerifier<LucerneSearchDocumentAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences.Union(ReferencesHelper.CMSSearchLucerne3References).ToArray();
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void InputWithIncident_MethodReturnValue_SurfacesDiagnostic()
        {
            var test = @"using CMS.Search.Lucerne3; 

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
    {{
        private LuceneSearchDocument Method()
        {{
            return new LuceneSearchDocument();
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCERNE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "LuceneSearchDocument", "IsearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 6, 17)}
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [Test]
        public void InputWithIncident_VariableDeclaration_SurfacesDiagnostic()
        {
            var test = @"using CMS.Search.Lucerne3; 

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method()
        {
            LuceneSearchDocument badVariable = null;
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCERNE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "LuceneSearchDocument", "IsearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 17) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [Test]
        public void InputWithIncident_MethodParameter_SurfacesDiagnostic()
        {
            var test = @"using CMS.Search.Lucerne3; 

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method(LuceneSearchDocument doc)
        {
            // Do nothing
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCERNE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "LuceneSearchDocument", "IsearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 6, 38) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);
        }

        [Test]
        public void OkayUsage_TypeofArgument_NoDiagnostic()
        {
            var test = @"using CMS.Search.Lucerne3; 

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method()
        {
            var type = typeof(LuceneSearchDocument);
            var luceneSearchDocument = new LuceneSearchDocument();
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }
    }
}