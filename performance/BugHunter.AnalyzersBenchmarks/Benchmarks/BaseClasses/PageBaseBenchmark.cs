using System.Linq;
using BenchmarkDotNet.Attributes;
using BugHunter.Analyzers.BenchmarkingBaselineAnalyzers;
using BugHunter.TestUtils;
using BugHunter.TestUtils.Helpers;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SampleProjectGenerator.CodeGenerators.BaseClassRules.Implementation;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.BaseClasses
{
    public class PageBaseBenchmark
    {
        private readonly DiagnosticAnalyzer _v0 = new SyntaxNodeClassDeclarationBaselineAnalyzer();
        private readonly DiagnosticAnalyzer _v1 = new PageBaseAnalyzer();

        private readonly MetadataReference[] _additionalReferences;
        private readonly string[] _sources;
        private readonly FakeFileInfo _fakeFileInfo;

        public PageBaseBenchmark()
        {
            _additionalReferences = ReferencesHelper.CMSBasicReferences
                .Union(new[] { ReferencesHelper.CMSBaseWebUI, ReferencesHelper.SystemWebReference, ReferencesHelper.SystemWebUIReference, ReferencesHelper.CMSUiControls })
                .ToArray();

            var codeGenerator = new PageBase();
            _sources = codeGenerator.GenerateClasses(10, 2);
            _fakeFileInfo = codeGenerator.GetFakeFileInfo(0);
        }

        [Benchmark(Baseline = false)]
        public int FilesCompilation()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences);
        }

        [Benchmark(Baseline = true)]
        public int AnalyzerV0_EmptyCallback()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v0);
        }

        [Benchmark]
        public int AnalyzerV1_SyntxNodeRegistered()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _fakeFileInfo, _v1);
        }
    }
}