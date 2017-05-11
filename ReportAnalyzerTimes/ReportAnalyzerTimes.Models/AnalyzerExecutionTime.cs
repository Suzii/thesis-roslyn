using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ReportAnalyzerTimes.Models
{
    /// <summary>
    /// Structure grouping the analyzer name and its execution time
    /// </summary>
    public struct AnalyzerExecutionTime
    {
        private static readonly Regex LineRegex = new Regex(@"([\d,]+) s\s+- ([\w.]+)", RegexOptions.Compiled);

        /// <summary>
        /// Name of the analyzer the <see cref="ExecutionTime"/> belongs
        /// </summary>
        public string AnalyzerName { get; set; }
        
        /// <summary>
        /// Execution time of the <see cref="AnalyzerName"/>
        /// </summary>
        public double ExecutionTime { get; set; }

        /// <summary>
        /// Sums up two analyzer execution times of a same name. Trows otherwise.
        /// </summary>
        /// <param name="time1">First analyzer execution time</param>
        /// <param name="time2">Second analyzer execution time</param>
        /// <returns>Summed execution times with the same name</returns>
        public static AnalyzerExecutionTime operator +(AnalyzerExecutionTime time1, AnalyzerExecutionTime time2)
        {
            if (time1.AnalyzerName != time2.AnalyzerName)
            {
                throw new AggregateException("Plus operator can only be applied to analyzers with same name");
            }

            return new AnalyzerExecutionTime
            {
                AnalyzerName = time1.AnalyzerName,
                ExecutionTime = time1.ExecutionTime + time2.ExecutionTime
            };
        }

        /// <summary>
        /// Returns a string representation in for of 'time unit - analyzer name'
        /// </summary>
        /// <returns>Analyzer execution time string representation</returns>
        public override string ToString()
        {
            return $"{ExecutionTime:##0.000 s} \t\t- {AnalyzerName}";
        }

        /// <summary>
        /// Parses the string representation into the <see cref="AnalyzerExecutionTime"/> structure
        /// </summary>
        /// <param name="line">Line to be parsed, needs to be in a same format thet the ToSting() method produces</param>
        /// <returns>AnalyzerExecutionTime structure parsed from <param name="line"></param> data</returns>
        public static AnalyzerExecutionTime FromString(string line)
        {
            var match = LineRegex.Match(line).Groups;

            string analyzerName = match[2].Value;
            double executionTime = double.Parse(match[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture);

            return new AnalyzerExecutionTime
            {
                AnalyzerName = analyzerName,
                ExecutionTime = executionTime
            };
        }
    }
}