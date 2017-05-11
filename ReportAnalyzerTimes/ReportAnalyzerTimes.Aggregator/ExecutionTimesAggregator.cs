using System;
using System.Collections.Generic;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    internal class ExecutionTimesAggregator
    {
        private IDictionary<string, IList<double>> _executionTimesAggregator = new Dictionary<string, IList<double>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds single <param name="analyzerExecutionTime"></param> to the internal collection
        /// </summary>
        /// <param name="analyzerExecutionTime"></param>
        public void AddAnalyzerExecutionTime(AnalyzerExecutionTime analyzerExecutionTime)
        {
            if (!_executionTimesAggregator.ContainsKey(analyzerExecutionTime.AnalyzerName))
            {
                _executionTimesAggregator[analyzerExecutionTime.AnalyzerName] = new List<double>();
            }

            _executionTimesAggregator[analyzerExecutionTime.AnalyzerName].Add(analyzerExecutionTime.ExecutionTime);
        }

        /// <summary>
        /// Returns the aggregated results of analyzer execution times
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AnalyzerExecutionTimes> GetAggregatedResults()
        {
            return _executionTimesAggregator.Select(pair => new AnalyzerExecutionTimes
            {
                AnalyzerName = pair.Key,
                ExecutionTimes = pair.Value
            });
        }

        /// <summary>
        /// Resets the aggregator
        /// </summary>
        public void Reset()
        {
            _executionTimesAggregator = new Dictionary<string, IList<double>>(StringComparer.OrdinalIgnoreCase);
        }
    }
}