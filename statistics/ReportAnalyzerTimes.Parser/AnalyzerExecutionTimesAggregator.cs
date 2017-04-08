using System.Collections.Generic;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Parser
{
    internal class AnalyzerExecutionTimesAggregator
    {
        /// <summary>
        /// Merges the per project results of analyzer execution times into a collection, 
        /// in which each analyzer has time that corresponds to the sum of times in each project
        /// </summary>
        /// <param name="analyzerExecutionTimesPerProject"></param>
        /// <returns></returns>
        public IEnumerable<AnalyzerExecutionTime> GetAggregatedResultsPerAnalyzer(IEnumerable<IEnumerable<AnalyzerExecutionTime>> analyzerExecutionTimesPerProject)
        {
            var aggregatedExecutionTimes = new Dictionary<string, double>();
            foreach (var executionTimesForProject in analyzerExecutionTimesPerProject)
            {
                foreach (var singleExecutionTime in executionTimesForProject)
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