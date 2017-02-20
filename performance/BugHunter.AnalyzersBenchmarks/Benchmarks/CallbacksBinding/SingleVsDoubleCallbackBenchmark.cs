using BenchmarkDotNet.Attributes;
using BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers;
using BugHunter.TestUtils.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SampleProjectGenerator.CodeGenerators.BasicOperations;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.CallbacksBinding
{
    public class SingleVsDoubleCallbackBenchmark
    {
        private readonly DiagnosticAnalyzer _v0 = new MemberAccessEmptyCallbacksAnalyzer();
        private readonly DiagnosticAnalyzer _v1 = new MemberAccessSingleCallbackAnalyzer();
        private readonly DiagnosticAnalyzer _v2 = new MemberAccessTwoCallbacksAnalyzer();

        private readonly MetadataReference[] _additionalReferences;
        private readonly string[] _sources;
        private readonly FakeFileInfo _fakeFileInfo;

        public SingleVsDoubleCallbackBenchmark()
        {
            _additionalReferences = new MetadataReference[] {};

            var codeGenerator = new MemberAccessesGenerator();
            _sources = codeGenerator.GenerateClasses(2000, 10);
            _fakeFileInfo = codeGenerator.GetFakeFileInfo(0);
        }

        [Benchmark]
        public int AnalyzerV0_EmptyCallback()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v0);
        }

        [Benchmark]
        public int AnalyzerV1_SingleCallback()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v1);
        }

        [Benchmark]
        public int AnalyzerV2_TwoCallbacks()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v2);
        }
    }
}