using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace BugHunter.AnalyzersBenchmarks.Configuration
{
    public abstract class BaseConfig : ManualConfig
    {
        protected BaseConfig(Job job)
        {
            KeepBenchmarkFiles = true;
            Add(job);
            Add(
                RankColumn.Arabic,
                TargetMethodColumn.Method,
                StatisticColumn.Mean,
                StatisticColumn.StdErr,
                StatisticColumn.StdDev,
                StatisticColumn.Min,
                StatisticColumn.Max,
                StatisticColumn.OperationsPerSecond);

            Add(HtmlExporter.Default, MarkdownExporter.GitHub);
        }
    }
}