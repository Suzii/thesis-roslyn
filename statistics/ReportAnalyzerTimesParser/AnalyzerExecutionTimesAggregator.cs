using System.Collections.Generic;
using System.Linq;

namespace ReportAnalyzerTimesParser
{
    class AnalyzerExecutionTimesAggregator
    {
        public IEnumerable<AnalyzerExecutionTime> GetAggregatedResultsPerAnalyzer(IEnumerable<IEnumerable<AnalyzerExecutionTime>> analyzerExecutionTimesPerProject)
        {
            var aggregatedExecutionTimes = new Dictionary<string, double>();
            foreach (var executionTimesForPoject in analyzerExecutionTimesPerProject)
            {
                foreach (var singleExecutionTime in executionTimesForPoject)
                {
                    double currentTime;
                    var wasFound = aggregatedExecutionTimes.TryGetValue(singleExecutionTime.AnalyzerName, out currentTime);
                    var aggregatedExecutionTime = singleExecutionTime.ExecutionTime + (wasFound ? currentTime : 0);
                    aggregatedExecutionTimes[singleExecutionTime.AnalyzerName] = aggregatedExecutionTime;
                }
            }

            var result = aggregatedExecutionTimes
                .Select(pair => new AnalyzerExecutionTime { AnalyzerName = pair.Key, ExecutionTime = pair.Value })
                .ToList();

            result.Sort((time1, time2) => time2.ExecutionTime.CompareTo(time1.ExecutionTime));

            return result;
        }
    }
}