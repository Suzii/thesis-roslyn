// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace ReportAnalyzerTimes.Models
{
    /// <summary>
    /// Groups the analyzer name and its execution times
    /// </summary>
    public struct AnalyzerExecutionTimes
    {
        /// <summary>
        /// Gets or sets name of the analyzer the times belong to
        /// </summary>
        public string AnalyzerName { get; set; }

        /// <summary>
        /// Gets or sets times of the analyzer execution
        /// </summary>
        public IList<double> ExecutionTimes { get; set; }
    }
}