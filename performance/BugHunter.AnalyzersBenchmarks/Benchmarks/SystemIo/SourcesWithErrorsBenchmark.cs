using System.Linq;
using BugHunter.TestUtils;
using Microsoft.CodeAnalysis;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo
{
    public class SourcesWithErrorsBenchmark : BaseSystemIoBenchmarks
    {
        protected override MetadataReference[] AdditionalReferences { get; }
        protected override string[] Sources { get; }

        public SourcesWithErrorsBenchmark()
        {
            AdditionalReferences = ReferencesHelper.CMSBasicReferences.Union(
                ReferencesHelper.GetReferencesFor(
                    typeof(CMS.Search.ISearchProvider),
                    typeof(Lucene.Net.Search.BooleanClause),
                    typeof(WorldNet.Net.SynExpand),
                    typeof(CMS.Search.Lucene3.LuceneSearchDocument)
                ))
                .ToArray();

            Sources = new SampleProjectGenerator.CodeGenerators.ConsoleApp.Implementation.SystemIo().GenerateClasses(100, 10);
        }
    }
}
