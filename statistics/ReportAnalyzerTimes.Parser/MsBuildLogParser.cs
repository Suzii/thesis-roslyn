using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Parser
{
    class MsBuildLogParser
    {
        // matches any number of lines for analyzer times
        private static readonly Regex ExecutionTimesForProjectRegex = new Regex(@"Total analyzer execution time: (.*) seconds(?:.*\n){1,4}\s*Time(?:.*\n){2}((?:.*\n){1,}?)\s+Output Item\(s\):", RegexOptions.Multiline);

        private static readonly Regex ExecutionTimePerAnalyzerRegex = new Regex(@"[\s]*[<]?((?:\d+)\.(?:\d+))[^B]*([\S]+)", RegexOptions.Compiled);

        public IEnumerable<IEnumerable<AnalyzerExecutionTime>> GetAnalyzerExecutionTimesForProjects(string logContents)
        {
            var executionTimesPerProject = ExecutionTimesForProjectRegex
                .Matches(logContents)
                .Cast<Match>()
                .Select(match => match.Groups[2])
                .Select(group => group.Value)
                .ToList();

            var analyzerExecutionTimesPerProject = executionTimesPerProject
                .Select(GetAnalyzerExecutionTimes)
                .ToList();

            return analyzerExecutionTimesPerProject;
        }

        private static IEnumerable<AnalyzerExecutionTime> GetAnalyzerExecutionTimes(string projectExecutionTimes)
        {
            return ExecutionTimePerAnalyzerRegex
                .Matches(projectExecutionTimes)
                .Cast<Match>()
                .Select(projectExecutionTimeMatch
                    => new AnalyzerExecutionTime
                    {
                        ExecutionTime =
                            double.Parse(projectExecutionTimeMatch.Groups[1].Value, NumberStyles.AllowDecimalPoint,
                                NumberFormatInfo.InvariantInfo),
                        AnalyzerName = projectExecutionTimeMatch.Groups[2].Value
                    });
        }
    }
}