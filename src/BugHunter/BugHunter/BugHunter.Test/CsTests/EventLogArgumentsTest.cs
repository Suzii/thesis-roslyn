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
    public class EventLogArgumentsTest : CodeFixVerifier
    {
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new EventLogArgumentsCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EventLogArgumentsAnalyzer();
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

        [TestCase("\"I\"", "EventType.INFORMATION")]
        [TestCase("\"W\"", "EventType.WARNING")]
        [TestCase("\"E\"", "EventType.ERROR")]
        public void InputWithWrongArgument_SurfacesDiagnostic(string oldArgument, string newArgument)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class BH1001LogEventEventTypeShoulNotBeHardcoded
    {{
        public void SampleMethod()
        {{
            CMS.EventLog.EventLogProvider.LogEvent({oldArgument}, ""source"", ""eventCode"", ""eventDescription"");
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = "BH1001",
                Message = $"LogEvent called with event type \"{oldArgument}\".",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 52) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.EventLog;
namespace SampleTestProject.CsSamples
{{
    public class BH1001LogEventEventTypeShoulNotBeHardcoded
    {{
        public void SampleMethod()
        {{
            CMS.EventLog.EventLogProvider.LogEvent({newArgument}, ""source"", ""eventCode"", ""eventDescription"");
        }}
    }}
}}";

            VerifyCSharpFix(test, expectedFix);
        }

        [TestCase("\"S\"")]
        [TestCase("CMS.EventLog.EventType.INFORMATION")]
        [TestCase("CMS.EventLog.EventType.WARNING")]
        [TestCase("CMS.EventLog.EventType.ERROR")]
        public void InputWithOkArgument_NoDiagnostic(string oldArgument)
        {
            var test = $@"
namespace SampleTestProject.CsSamples
{{
    public class BH1001LogEventEventTypeShoulNotBeHardcoded
    {{
        public void SampleMethod()
        {{
            CMS.EventLog.EventLogProvider.LogEvent({oldArgument}, ""source"", ""eventCode"", ""eventDescription"");
        }}
    }}
}}";
            VerifyCSharpDiagnostic(test);
        }
    }
}