using BugHunter.InternalGuidelinesRules.Analyzers;
using BugHunter.InternalGuidelinesRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace BugHunter.Test.InternalGuidelines
{
    [TestFixture]
    public class EventLogArgumentsTest : CodeFixVerifier<EventLogArgumentsAnalyzer, EventLogArgumentsCodeFixProvider>
    {
        protected override MetadataReference[] GetAdditionalReferences()
        {
            return ReferencesHelper.BasicReferences;
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
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            CMS.EventLog.EventLogProvider.LogEvent({oldArgument}, ""source"", ""eventCode"", ""eventDescription"");
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIds.EVENT_LOG_ARGUMENTS,
                Message = $"LogEvent called with event type '{oldArgument}'.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 8, 52) }
            };

            VerifyCSharpDiagnostic(test, expectedDiagnostic);

            var expectedFix = $@"using CMS.EventLog;

namespace SampleTestProject.CsSamples
{{
    public class SampleClass
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
    public class SampleClass
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