using System.Collections.Generic;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    interface IResultsExporter
    {
        /// <summary>
        /// Exports the <param name="analyzersExecutionTimes"></param> into file
        /// </summary>
        /// <param name="filePath">File path to which results should be exported</param>
        /// <param name="analyzersExecutionTimes">Execution times of analyzers to be exported</param>
        void Export(string filePath, IEnumerable<AnalyzerExecutionTimes> analyzersExecutionTimes);
    }
}