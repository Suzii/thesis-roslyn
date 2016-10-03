using System;
using BugHunter.CsRules.Analyzers;
using BugHunter.CsRules.CodeFixes;
using BugHunter.Test.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BugHunter.Test.CsTests
{
    [TestClass]
    public class BH1001Test : CodeFixVerifier
    {
        private readonly Type[] _dependentTypes = {
            typeof(CMS.Core.ModuleName),
            typeof(CMS.Base.BaseModule),
            typeof(CMS.DataEngine.TypeCondition),
            typeof(CMS.Helpers.AJAXHelper),
            typeof(CMS.IO.AbstractFile),
            typeof(CMS.EventLog.EventLogProvider)
        };

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new BH1001CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new BH1001EventLogArguments();
        }

        [TestMethod]
        public void EmptyInput_NoDiagnostic()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestMethod]
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

            VerifyCSharpDiagnostic(test, _dependentTypes, expectedDiagnostic);

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

            VerifyCSharpFix(test, expectedFix, _dependentTypes);
        }
    }
}