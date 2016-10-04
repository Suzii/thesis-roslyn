using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace BugHunter.Test.CsTests
{
    [TestFixture]
    public class BH1001Test : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new BH1001CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new BH1001EventLogArguments();
        }

        protected override MetadataReference[] GetAdditionalReferences()
        {
            return Constants.BasicReferences;
        }

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [Test]
        public void InputWithInfoString_SurfacesDiagnostic()
        {
            var test = @"
namespace SampleTestProject.CsSamples
{
    public class BH1001LogEventEventTypeShoulNotBeHardcoded
    {
        public void SampleMethod()
        {
            CMS.EventLog.EventLogProvider.LogEvent(""I"", ""source"", ""eventCode"", ""eventDescription"");
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1001",
                Message = "LogEvent called with event type \"\"I\"\".",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 8, 13)
                        }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = @"
namespace SampleTestProject.CsSamples
{
    public class BH1001LogEventEventTypeShoulNotBeHardcoded
    {
        public void SampleMethod()
        {
            CMS.EventLog.EventLogProvider.LogEvent(CMS.EventLog.EventType.INFORMATION, ""source"", ""eventCode"", ""eventDescription"");
        }
    }
}";

            VerifyCSharpFix(test, expectedFix);
        }
    }
}