// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    /// <summary>
    /// Class that exports analyzers execution files into a CSV file
    /// </summary>
    public class CsvResultsExporter : IResultsExporter
    {
        /// <summary>
        /// Exports the <paramref name="analyzersExecutionTimes" /> into the <paramref name="filePath" />/> file
        /// </summary>
        /// <param name="filePath">File to put the results into</param>
        /// <param name="analyzersExecutionTimes">Analyzers execution times to be exported</param>
        public void Export(string filePath, IEnumerable<AnalyzerExecutionTimes> analyzersExecutionTimes)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.AppendAllLines(filePath, GetAllLines(analyzersExecutionTimes));
        }

        private IEnumerable<string> GetAllLines(IEnumerable<AnalyzerExecutionTimes> analyzersExecutionTimes)
        {
            return analyzersExecutionTimes.Select(GetSingleLine);
        }

        private string GetSingleLine(AnalyzerExecutionTimes analyzerExecutionTimes)
        {
            return analyzerExecutionTimes.AnalyzerName + ";" + string.Join(";", analyzerExecutionTimes.ExecutionTimes);
        }
    }
}