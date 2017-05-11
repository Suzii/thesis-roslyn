using System.Collections.Generic;

namespace ReportAnalyzerTimes.Models
{
    /// <summary>
    /// Groups the analyzer name and its execution times
    /// </summary>
    public struct AnalyzerExecutionTimes
    {
        /// <summary>
        /// Name of the analyzer the times belong to
        /// </summary>
        public string AnalyzerName { get; set; }

        /// <summary>
        /// Times of the analyzer execution
        /// </summary>
        public IList<double> ExecutionTimes { get; set; }
    }
}