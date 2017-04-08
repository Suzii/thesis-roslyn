using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ReportAnalyzerTimes.Models
{
    public struct AnalyzerExecutionTime
    {
        private static readonly Regex LineRegex = new Regex(@"([\d,]+) s\s+- ([a-zA-z.]+)", RegexOptions.Compiled);

        public string AnalyzerName { get; set; }
        public double ExecutionTime { get; set; }

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

        public override string ToString()
        {
            return $"{ExecutionTime:##0.000 s} \t\t- {AnalyzerName}";
        }

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