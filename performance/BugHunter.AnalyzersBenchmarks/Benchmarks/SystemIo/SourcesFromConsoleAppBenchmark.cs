using System;
using System.Collections.Generic;
using System.Linq;
using BugHunter.TestUtils;
using Microsoft.CodeAnalysis;
using SampleProjectGenerator.CodeGenerators;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo
{
    public class SourcesFromConsoleAppBenchmark : BaseSystemIoBenchmarks
    {
        protected override MetadataReference[] AdditionalReferences { get; }

        protected override string[] Sources { get; }

        public SourcesFromConsoleAppBenchmark()
        {
            AdditionalReferences = ReferencesHelper.CMSBasicReferences.Union(
                ReferencesHelper.GetReferencesFor(
                    typeof(CMS.Search.ISearchProvider),
                    typeof(Lucene.Net.Search.BooleanClause),
                    typeof(WorldNet.Net.SynExpand),
                    typeof(CMS.Search.Lucene3.LuceneSearchDocument)
                ))
                .ToArray();

            Sources = GetAllConsoleAppClassCodeGenerators().SelectMany(generator => generator.GenerateClasses(100, 10)).ToArray();
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
    }
}
