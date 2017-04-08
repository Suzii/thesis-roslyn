using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Aggregator
{
    public class CsvResultsExporter : IResultsExporter
    {
        public void Export(string filePath, IEnumerable<AnalyzerExecutionTimes> analyzersExecutionTimes)
        {
            File.AppendAllLines(filePath, GetAllLines(analyzersExecutionTimes));
        }

        private IEnumerable<string> GetAllLines(IEnumerable<AnalyzerExecutionTimes> analyzersExecutionTimes)
        {
            return analyzersExecutionTimes.Select(GetSingleLine);
        }

        private string GetSingleLine(AnalyzerExecutionTimes analyzerExecutionTimes)
        {
            return analyzerExecutionTimes.AnalyzerName+ ";" + string.Join(";", analyzerExecutionTimes.ExecutionTimes);
        }
    }
}