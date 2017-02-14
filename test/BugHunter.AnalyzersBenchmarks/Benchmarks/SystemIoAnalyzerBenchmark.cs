using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BugHunter.Analyzers.SystemIoRules.Analyzers;
using BugHunter.TestUtils.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks
{
    [HtmlExporter]
    [MemoryDiagnoser]
    public class SystemIoAnalyzerBenchmark
    {
        private readonly DiagnosticAnalyzer systemIoV1 = new SystemIOAnalyzer();
        // TODO
        private readonly DiagnosticAnalyzer systemIoV2 = new SystemIOAnalyzer();


        private readonly Document[] _documentsWithNoDiagnostic;
        private readonly Document[] _documentsWith1000Diagnostics;

        public SystemIoAnalyzerBenchmark()
        {
            var sourceWithNoDiagnostic = File.ReadAllText(@"C:\Users\zuzanad\code\thesis\thesis-sample-test-project\SampleProject\ConsoleApp\SystemIo.cs");
            var sourceWith1000Diagnostics = File.ReadAllText(@"C:\Users\zuzanad\code\thesis\thesis-sample-test-project\SampleProject\ConsoleApp\StringIndexOfMethod.cs");

            _documentsWithNoDiagnostic = ProjectCompilation.GetDocuments(new[]{ sourceWithNoDiagnostic }, null, null);
            _documentsWith1000Diagnostics = ProjectCompilation.GetDocuments(new[]{ sourceWith1000Diagnostics }, null, null);
        }


        [Benchmark]
        public void V1NoDiagnostic()
        {
            AnalyzerExecution.GetSortedDiagnosticsFromDocuments(systemIoV1, _documentsWithNoDiagnostic);
        }

        [Benchmark]
        public void V11000Diagnostic()
        {
            AnalyzerExecution.GetSortedDiagnosticsFromDocuments(systemIoV1, _documentsWith1000Diagnostics);
        }
    }
}
