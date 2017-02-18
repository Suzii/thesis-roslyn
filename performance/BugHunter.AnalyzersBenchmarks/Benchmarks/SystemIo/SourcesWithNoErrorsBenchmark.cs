using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BugHunter.Analyzers.BenchmarkingBaselineAnalyzers;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo
{
    public class SourcesWithNoErrorsBenchmark
    {
        private readonly DiagnosticAnalyzer _systemIoV0 = new SyntaxNodeIdentifierNameBaselineAnalyzer();
        private readonly DiagnosticAnalyzer _systemIoV2 = new SystemIOAnalyzer_V2_IdentifierNameAndSymbolAnalysis();
        private readonly DiagnosticAnalyzer _systemIoV4 = new SystemIOAnalyzer_V4_CompilationStart();
        private readonly DiagnosticAnalyzer _systemIoV5 = new SystemIOAnalyzer_V5_CompilationStartWithBagForDiagnosedNodes();
        private readonly DiagnosticAnalyzer _systemIoV6 = new SystemIOAnalyzer_V6_CompilationStartAndSyntaxTree();
        private readonly DiagnosticAnalyzer _systemIoV7 = new SystemIOAnalyzer_V7_CompilationStartAndSyntaxTreeAndFulltextSearch();

        private readonly MetadataReference[] _additionalReferences;
        private readonly string[] _sources;

        public SourcesWithNoErrorsBenchmark()
        {
            _additionalReferences = ReferencesHelper.CMSBasicReferences.Union(
                ReferencesHelper.GetReferencesFor(
                    typeof(CMS.Search.ISearchProvider),
                    typeof(Lucene.Net.Search.BooleanClause),
                    typeof(WorldNet.Net.SynExpand),
                    typeof(CMS.Search.Lucene3.LuceneSearchDocument)
                ))
                .ToArray();

            _sources = Directory.GetFiles(Path.Combine(Constants.PATH_TO_SAMPLE_PROJECT, @"ConsoleApp"))
                .Where(fileName => fileName.EndsWith(".cs"))
                .Where(fileName => !fileName.Contains("SystemIo"))
                .Take(10)
                .Select(File.ReadAllText)
                .ToArray();
        }

        [Benchmark(Baseline = false)]
        public int FilesCompilation()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences);
        }

        [Benchmark(Baseline = true)]
        public int AnalyzerV0_EmptyCallback()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _systemIoV0);
        }

        [Benchmark]
        public int AnalyzerV1_SyntxNodeRegistered()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _systemIoV2);
        }

        [Benchmark]
        public int AnalyzerV4_CompilationStartAndSyntaxNode()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _systemIoV4);
        }

        [Benchmark]
        public int AnalyzerV5_CompilationStartSyntaxNodeAndCompilationEnd()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _systemIoV5);
        }

        [Benchmark]
        public int AnalyzerV7_CompilationStartAndSyntaxTree()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _systemIoV6);
        }

        [Benchmark]
        public int AnalyzerV7_CompilationStartAndSyntaxTreeAndFulltextSearch()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, _systemIoV7);
        }
    }
}
