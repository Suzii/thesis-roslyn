using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

namespace BugHunter.AnalyzersBenchmarks.Configuration
{
    public class DevelopmentConfig : BaseConfig
    {
        public DevelopmentConfig() : base(new Job("DevelopmentBenchmark")
        {
            Run =
            {
                RunStrategy = RunStrategy.ColdStart,
                LaunchCount = 4,
                TargetCount = 2,
                WarmupCount = 5,
                UnrollFactor = 4,
                InvocationCount = 4,
            }
        })
        {
        }
    }
}