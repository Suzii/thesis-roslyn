using System.Linq;
using BugHunter.Core.Tests.DiagnosticsFormatting;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Services;
using BugHunter.TestUtils.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace BugHunter.Core.Tests.Analyzers
{
    [TestFixture]
    public class ApiReplacementForMemberAnalyzerTests : CodeFixVerifier<FakeApiReplacementForMemberAnalyzer>
    {
        protected override MetadataReference[] GetAdditionalReferences() => null;

        private static DiagnosticResult CreateDiagnosticResult(params object[] messageArgs)
            => new DiagnosticResult
            {
                Id = "BHFAKE",
                Message = string.Format(@"'{0}' should not be used.", messageArgs),
                Severity = DiagnosticSeverity.Warning,
            };

        private static string _fakeClassSource = @"
namespace OkayNamespace
{
    public class FakeClass
    {
        public string FakeMember { get; set; }

        public FakeClass Create()
        {
            return new FakeClass();
        }
    }
}

namespace FakeNamespace
{
    public class FakeClass
    {
        public string FakeMember { get; set; }

        public FakeClass Create()
        {
            return new FakeClass();
        }
    }
}";

        [Test]
        public void EmptyInput_NoDiagnostic()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("instance")]
        [TestCase("new SampleClass()")]
        [TestCase("new OkayNamespace.FakeClass()")]
        [TestCase("instance.Create()")]
        public void OkInput_NoDiagnostic(string accessedInstance)
        {
            var test = $@"using OkayNamespace;
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public string FakeMember {{ get; set; }}

        public void SampleMethod()
        {{
            var instance = new FakeClass();
            var a1 = {accessedInstance}.FakeMember;
            var a2 = {accessedInstance}?.FakeMember;
            var b1 = {accessedInstance}.FakeMember.Substring(0);
            var b2 = {accessedInstance}.FakeMember?.Substring(0);
            var b3 = {accessedInstance}?.FakeMember.Substring(0);
            var b4 = {accessedInstance}?.FakeMember?.Substring(0);
        }}
    }}
}}";

            VerifyCSharpDiagnostic(new[] { test, _fakeClassSource });
        }

        [TestCase("instance")]
        [TestCase("instance.Create()")]
        [TestCase("instance.Create().Create()")]
        [TestCase("new FakeClass()")]
        [TestCase("new FakeClass().Create()")]
        [TestCase("new FakeNamespace.FakeClass()")]
        public void InputWithIncident_MemberAccesses_SurfacesDiagnostic(string accessedInstance)
        {
            var test = $@"using FakeNamespace;
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var instance = new FakeClass();
            var a1 = {accessedInstance}.FakeMember;
            var a2 = {accessedInstance}?.FakeMember;

            var b1 = {accessedInstance}.FakeMember.Substring(0);
            var b2 = {accessedInstance}.FakeMember?.Substring(0);
            var b3 = {accessedInstance}?.FakeMember.Substring(0);
            var b4 = {accessedInstance}?.FakeMember?.Substring(0);
        }}
    }}
}}";

            VerifyCSharpDiagnostic(
                new[] { test, _fakeClassSource },
                CreateDiagnosticResult($"{accessedInstance}.FakeMember").WithLocation(9, 22),
                CreateDiagnosticResult($"{accessedInstance}?.FakeMember").WithLocation(10, 22),
                CreateDiagnosticResult($"{accessedInstance}.FakeMember").WithLocation(12, 22),
                CreateDiagnosticResult($"{accessedInstance}.FakeMember").WithLocation(13, 22),
                CreateDiagnosticResult($"{accessedInstance}?.FakeMember").WithLocation(14, 22),
                CreateDiagnosticResult($"{accessedInstance}?.FakeMember").WithLocation(15, 22));
        }

        [TestCase("instance?.Create()")]
        [TestCase("instance?.Create()?.Create()")]
        public void InputWithIncident_PrecedingConditionalAccess_SurfacesDiagnostic(string accessedInstance)
        {
            var test = $@"using FakeNamespace;
namespace SampleTestProject.CsSamples 
{{
    public class SampleClass
    {{
        public void SampleMethod()
        {{
            var instance = new FakeClass();
            var a1 = {accessedInstance}.FakeMember;
            var a2 = {accessedInstance}?.FakeMember;

            var b1 = {accessedInstance}.FakeMember.Substring(0);
            var b2 = {accessedInstance}.FakeMember?.Substring(0);
            var b3 = {accessedInstance}?.FakeMember.Substring(0);
            var b4 = {accessedInstance}?.FakeMember?.Substring(0);
        }}
    }}
}}";
            var sources = new[] { test, _fakeClassSource };

            var documents = ProjectCompilation.GetDocuments(sources, null, null);
            var analyzer = GetCSharpDiagnosticAnalyzer();
            var diagnostics = AnalyzerExecution.GetSortedDiagnosticsFromDocuments(analyzer, documents);

            Assert.AreEqual(6, diagnostics.Length);

            AssertBestEfforDiagnostic(analyzer, CreateDiagnosticResult(string.Empty).WithMessage(".FakeMember").WithLocation(9, 22), diagnostics.ElementAt(0));
            AssertBestEfforDiagnostic(analyzer, CreateDiagnosticResult(string.Empty).WithMessage(".FakeMember").WithLocation(10, 22), diagnostics.ElementAt(1));
            AssertBestEfforDiagnostic(analyzer, CreateDiagnosticResult(string.Empty).WithMessage(".FakeMember").WithLocation(12, 22), diagnostics.ElementAt(2));
            AssertBestEfforDiagnostic(analyzer, CreateDiagnosticResult(string.Empty).WithMessage(".FakeMember").WithLocation(13, 22), diagnostics.ElementAt(3));
            AssertBestEfforDiagnostic(analyzer, CreateDiagnosticResult(string.Empty).WithMessage(".FakeMember").WithLocation(14, 22), diagnostics.ElementAt(4));
            AssertBestEfforDiagnostic(analyzer, CreateDiagnosticResult(string.Empty).WithMessage(".FakeMember").WithLocation(15, 22), diagnostics.ElementAt(5));
        }

        private void AssertBestEfforDiagnostic(DiagnosticAnalyzer analyzer, DiagnosticResult expected, Diagnostic actual)
        {
            if (expected.Line == -1 && expected.Column == -1)
            {
                Assert.AreEqual(Location.None, actual.Location,
                    $"Expected:\nA project diagnostic with No location\nActual:\n{FormatDiagnostics(analyzer, actual)}");
            }
            else
            {
                AssertLocation.IsWithinOnOneLine(expected.Locations.First(), actual.Location);
            }

            Assert.AreEqual(expected.Id, actual.Id,
                $"Expected diagnostic id to be \"{expected.Id}\" was \"{actual.Id}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");

            Assert.AreEqual(expected.Severity, actual.Severity,
                $"Expected diagnostic severity to be \"{expected.Severity}\" was \"{actual.Severity}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");

            Assert.That(
                actual.GetMessage().Contains(expected.Message),
                $"Expected diagnostic message \"{actual.GetMessage()}\" to contain \"{expected.Message}\"\r\n\r\nDiagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}\r\n");
        }
    }
}