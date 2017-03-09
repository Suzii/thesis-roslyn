using System.Linq;
using BenchmarkDotNet.Attributes;
using BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers;
using BugHunter.SystemIO.Analyzers.Analyzers;
using BugHunter.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo
{
    public class SourcesWithErrorsBenchmark
    {
        private readonly DiagnosticAnalyzer _systemIoV0 = new SyntaxNodeIdentifierNameBaselineAnalyzer();
        private readonly DiagnosticAnalyzer _systemIoV2 = new V02_IdentifierName_SymbolAnalysis();
        private readonly DiagnosticAnalyzer _systemIoV4 = new V04_CompilationStartAndIdentifierName_SymbolAnalysis();
        private readonly DiagnosticAnalyzer _systemIoV5 = new V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag();
        private readonly DiagnosticAnalyzer _systemIoV6 = new V06_CompilationStartAndSyntaxTree_LookForIdentifierNames();
        private readonly DiagnosticAnalyzer _systemIoV7 = new V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag();

        private readonly MetadataReference[] _additionalReferences;
        private readonly string[] _sources;

        public SourcesWithErrorsBenchmark()
        {
            _additionalReferences = ReferencesHelper.CMSBasicReferences.Union(
                ReferencesHelper.GetReferencesFor(
                    typeof(CMS.Search.ISearchProvider),
                    typeof(Lucene.Net.Search.BooleanClause),
                    typeof(WorldNet.Net.SynExpand),
                    typeof(CMS.Search.Lucene3.LuceneSearchDocument)
                ))
                .ToArray();

            _sources = new SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation.SystemIo().GenerateClasses(10, 10);
        }

        [Benchmark(Baseline = false)]
        public int FilesCompilation()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences);
        }

        [Benchmark(Baseline = true)]
        public int AnalyzerV0_EmptyCallback()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, null, _systemIoV0);
        }

        [Benchmark]
        public int AnalyzerV1_SyntxNodeRegistered()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, null, _systemIoV2);
        }

        [Benchmark]
        public int AnalyzerV4_CompilationStartAndSyntaxNode()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, null, _systemIoV4);
        }

        [Benchmark]
        public int AnalyzerV5_CompilationStartSyntaxNodeAndCompilationEnd()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, null, _systemIoV5);
        }
        
        [Benchmark]
        public int AnalyzerV6_CompilationStartAndSyntaxTree()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, null, _systemIoV6);
        }

        [Benchmark]
        public int AnalyzerV7_CompilationStartAndSyntaxTreeAndFulltextSearch()
        {
            return AnalysisRunner.RunAnalysis(_sources, _additionalReferences, null, _systemIoV7);
        }
    }
}
