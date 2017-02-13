using System.Linq;
using BugHunter.Analyzers.AbstractionOverImplementation.Analyzers;
using BugHunter.Analyzers.AbstractionOverImplementation.CodeFixes;
using BugHunter.Analyzers.Test.CmsApiReplacementsTests.Constants;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Analyzers.Test.AbstractionOverImplementationTests
{
    [TestFixture]
    public class LuceneSearchDocumentTest : CodeFixVerifier<LuceneSearchDocumentAnalyzer, LuceneSearchDocumentCodeFixProvider>
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
            var test = @"using CMS.Search.Lucene3; 

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private LuceneSearchDocument Method()
        {
            return new LuceneSearchDocument();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCENE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "LuceneSearchDocument", "ISearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {new DiagnosticResultLocation("Test0.cs", 7, 17)}
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS.Search.Lucene3;
using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private ISearchDocument Method()
        {
            return new LuceneSearchDocument();
        }
    }
}";

            VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithIncident_MethodReturnValue_FullyQualifiedName_SurfacesDiagnostic()
        {
            var test = @"namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private CMS.Search.Lucene3.LuceneSearchDocument Method()
        {
            return new CMS.Search.Lucene3.LuceneSearchDocument();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCENE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "CMS.Search.Lucene3.LuceneSearchDocument", "ISearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 17) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private ISearchDocument Method()
        {
            return new CMS.Search.Lucene3.LuceneSearchDocument();
        }
    }
}";

            VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void InputWithIncident_VariableDeclaration_SurfacesDiagnostic()
        {
            var test = @"using CMS.Search.Lucene3; 

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private bool Method()
        {
            LuceneSearchDocument badVariable = null;
            return badVariable == null;
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCENE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "LuceneSearchDocument", "ISearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 9, 13) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS.Search.Lucene3;
using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private bool Method()
        {
            ISearchDocument badVariable = null;
            return badVariable == null;
        }
    }
}";
            // allow new compiler diagnostics is true due to unnecessary using directive
            VerifyCSharpFix(test, expectedFix, 0, true);
        }

        [Test]
        public void InputWithIncident_VariableDeclaration_FullyQualifiedName_SurfacesDiagnostic()
        {
            var test = @"namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private bool Method()
        {
            CMS.Search.Lucene3.LuceneSearchDocument badVariable = null;
            return badVariable == null;
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCENE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "CMS.Search.Lucene3.LuceneSearchDocument", "ISearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 13) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private bool Method()
        {
            ISearchDocument badVariable = null;
            return badVariable == null;
        }
    }
}";

            VerifyCSharpFix(test, expectedFix, 0, true);
        }

        [Test]
        public void InputWithIncident_MethodParameter_SurfacesDiagnostic()
        {
            var test = @"using CMS.Search.Lucene3; 

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
                Id = DiagnosticIds.LUCENE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "LuceneSearchDocument", "ISearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 7, 29) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS.Search.Lucene3;
using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method(ISearchDocument doc)
        {
            // Do nothing
        }
    }
}";

            // allow new compiler diagnostics is true due to unnecessary using directive
            VerifyCSharpFix(test, expectedFix, 0, true);
        }

        [Test]
        public void InputWithIncident_MethodParameter_FullyQualifiedName_SurfacesDiagnostic()
        {
            var test = @"namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method(CMS.Search.Lucene3.LuceneSearchDocument doc)
        {
            // Do nothing
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.LUCENE_SEARCH_DOCUMENT,
                Message = string.Format(MessagesConstants.MESSAGE, "CMS.Search.Lucene3.LuceneSearchDocument", "ISearchDocument"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 5, 29) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"using CMS.DataEngine;

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method(ISearchDocument doc)
        {
            // Do nothing
        }
    }
}";

            VerifyCSharpFix(test, expectedFix);
        }

        [Test]
        public void OkayUsage_ObjectCreation_NoDiagnostic()
        {
            var test = @"using CMS.Search.Lucene3; 

namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method()
        {
            var luceneSearchDocument = new LuceneSearchDocument();
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        [Test]
        public void OkayUsage_ObjectCreation_FullyQualifiedName_NoDiagnostic()
        {
            var test = @"namespace SampleTestProject.CsSamples
{
    public class SampleClass
    {
        private void Method()
        {
            var luceneSearchDocument = new CMS.Search.Lucene3.LuceneSearchDocument();
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }
    }
}