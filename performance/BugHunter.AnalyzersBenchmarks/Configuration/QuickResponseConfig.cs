using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

namespace BugHunter.AnalyzersBenchmarks.Configuration
{
    public class QuickResponseConfig : BaseConfig
    {
        public QuickResponseConfig() : base(new Job("SuperFastBenchmark")
        {
            Run =
            {
                RunStrategy = RunStrategy.ColdStart,
                LaunchCount = 1,
                TargetCount = 2,
                WarmupCount = 0,
                UnrollFactor = 1,
                InvocationCount = 1,
            },
        })
        {
        }
    }
}