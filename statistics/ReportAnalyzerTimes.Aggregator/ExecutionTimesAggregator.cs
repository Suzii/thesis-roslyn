using System;
using System.Collections.Generic;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    internal class ExecutionTimesAggregator
    {
        private IDictionary<string, IList<double>> _executionTimesAggregator = new Dictionary<string, IList<double>>(StringComparer.OrdinalIgnoreCase);

        public void AddAnalyzerExecutionTime(AnalyzerExecutionTime analyzerExecutionTime)
        {
            if (!_executionTimesAggregator.ContainsKey(analyzerExecutionTime.AnalyzerName))
            {
                _executionTimesAggregator[analyzerExecutionTime.AnalyzerName] = new List<double>();
            }

            _executionTimesAggregator[analyzerExecutionTime.AnalyzerName].Add(analyzerExecutionTime.ExecutionTime);
        }

        public IEnumerable<AnalyzerExecutionTimes> GetAggregatedResults()
        {
            return _executionTimesAggregator.Select(pair => new AnalyzerExecutionTimes
            {
                AnalyzerName = pair.Key,
                ExecutionTimes = pair.Value
            });
        }

        public void Reset()
        {
            _executionTimesAggregator = new Dictionary<string, IList<double>>(StringComparer.OrdinalIgnoreCase);
        }
    }
}