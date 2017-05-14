// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Class for parsing the contents of aggregated analyzer execution times per solution
    /// </summary>
    internal class ExecutionTimesParser
    {
        /// <summary>
        /// Parses the lines and yields the analyzer executiontimes
        /// </summary>
        /// <param name="lines">Lines containing analyzer execution times</param>
        /// <returns>Analyzer execution times parsed from lines</returns>
        public IEnumerable<AnalyzerExecutionTime> ParseExecutionTimes(string[] lines)
        {
            return lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(AnalyzerExecutionTime.FromString);
        }
    }
}