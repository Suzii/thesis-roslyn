// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Export <see cref="AnalyzerExecutionTimes"/> into a file
    /// </summary>
    internal interface IResultsExporter
    {
        /// <summary>
        /// Exports the <paramref name="analyzersExecutionTimes" /> into file
        /// </summary>
        /// <param name="filePath">File path to which results should be exported</param>
        /// <param name="analyzersExecutionTimes">Execution times of analyzers to be exported</param>
        void Export(string filePath, IEnumerable<AnalyzerExecutionTimes> analyzersExecutionTimes);
    }
}