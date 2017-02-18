using BenchmarkDotNet.Jobs;

namespace BugHunter.AnalyzersBenchmarks.Configuration
{
    public class ProductionConfig : BaseConfig
    {
        public ProductionConfig() : base(new Job())
        {
        }
    }
}