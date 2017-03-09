using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BugHunter.AnalyzersBenchmarks.BenchmarkingBaselineAnalyzers;
using BugHunter.SystemIO.Analyzers.Analyzers;
using BugHunter.TestUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SampleProjectGenerator.CodeGenerators;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo
{
    public class SourcesFromConsoleAppBenchmark
    {
        private readonly DiagnosticAnalyzer _systemIoV0 = new SyntaxNodeIdentifierNameBaselineAnalyzer();
        private readonly DiagnosticAnalyzer _systemIoV2 = new V02_IdentifierName_SymbolAnalysis();
        private readonly DiagnosticAnalyzer _systemIoV5 = new V05_CompilationStartIdentifierNameAndEnd_SymbolAnalysis_WithBag();
        private readonly DiagnosticAnalyzer _systemIoV6 = new V06_CompilationStartAndSyntaxTree_LookForIdentifierNames();
        private readonly DiagnosticAnalyzer _systemIoV7 = new V07_CompilationStartSyntaxTreeAndEnd_FulltextSearchAndSymbolAnallysis_WithBag();

        private readonly MetadataReference[] _additionalReferences;
        private readonly string[] _sources;

        public SourcesFromConsoleAppBenchmark()
        {
            _additionalReferences = ReferencesHelper.CMSBasicReferences.Union(
                ReferencesHelper.GetReferencesFor(
                    typeof(CMS.Search.ISearchProvider),
                    typeof(Lucene.Net.Search.BooleanClause),
                    typeof(WorldNet.Net.SynExpand),
                    typeof(CMS.Search.Lucene3.LuceneSearchDocument)
                ))
                .ToArray();

            _sources = GetAllConsoleAppClassCodeGenerators().SelectMany(generator => generator.GenerateClasses(10, 2)).ToArray();
        }

        private static IEnumerable<IClassCodeGenerator> GetAllConsoleAppClassCodeGenerators()
        {
            var generatorInterface = typeof(IClassCodeGenerator);
            var codeGenerators = generatorInterface.Assembly
                .GetTypes()
                .Where(type => generatorInterface.IsAssignableFrom(type) && !type.IsAbstract)
                .Where(generator => generator.Namespace != null && generator.Namespace.Contains("ConsoleApp"))
                .Select(Activator.CreateInstance)
                .Cast<IClassCodeGenerator>();

            return codeGenerators;
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
