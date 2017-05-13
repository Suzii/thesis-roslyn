// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Helper class for aggregating <see cref="AnalyzerExecutionTime"/>
    /// </summary>
    internal class ExecutionTimesAggregator
    {
        private IDictionary<string, IList<double>> _executionTimesAggregator = new Dictionary<string, IList<double>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds single <paramref name="analyzerExecutionTime" /> to the internal collection
        /// </summary>
        /// <param name="analyzerExecutionTime">Analyzer execution time</param>
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
        /// <returns>Aggregated results</returns>
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