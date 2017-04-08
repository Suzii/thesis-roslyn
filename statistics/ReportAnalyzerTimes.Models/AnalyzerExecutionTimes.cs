using System.Collections.Generic;

namespace ReportAnalyzerTimes.Models
{
    public struct AnalyzerExecutionTimes
    {
        public string AnalyzerName { get; set; }
        public IList<double> ExecutionTimes { get; set; }
    }
}