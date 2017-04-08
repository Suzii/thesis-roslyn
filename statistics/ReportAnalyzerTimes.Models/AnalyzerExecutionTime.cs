using System;

namespace ReportAnalyzerTimes.Models
{
    public struct AnalyzerExecutionTime
    {
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
    }
}