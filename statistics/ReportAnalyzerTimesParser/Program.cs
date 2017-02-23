using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportAnalyzerTimesParser
{
    class Program
    {
        private static readonly Regex ExecutionTimesForProjectRegex = new Regex(@"Total analyzer execution time(.*\n){4}((?:.*\n){24})", RegexOptions.Multiline);
        private static readonly Regex ExecutionTimePerAnalyzerRegex = new Regex(@"[\s]*[<]?((?:\d+)\.(?:\d+))[^B]*([A-Za-z\.]+)", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            const string inputFilePath = @"C:\tmp\msbuild-output.txt";
            const string outputFilePath = @"C:\tmp\analyzers-execution-times.txt";
            const string aggregatedOutputFilePath = @"C:\tmp\analyzers-execution-times-aggregated.txt";
            
            var inputFileContent = string.Join(Environment.NewLine, File.ReadLines(inputFilePath));

            var executionTimesPerProject = ExecutionTimesForProjectRegex
                .Matches(inputFileContent)
                .Cast<Match>()
                .Select(match => match.Groups[2])
                .Select(group => group.Value)
                .ToList();

            var analyzerExecutionTimesPerProject = executionTimesPerProject
                .Select(GetAnalyzerExecutionTimes)
                .ToList();

            var aggregatedRusultsPerAnalyzer = AggregatedResultsPerAnalyzer(analyzerExecutionTimesPerProject);

            var resultsInFancyString = ResultsInFancyString(analyzerExecutionTimesPerProject);
            File.WriteAllText(outputFilePath, resultsInFancyString);

            var aggregatedResultsInFancyString = aggregatedRusultsPerAnalyzer.Select(time => time.ToString());
            File.WriteAllLines(aggregatedOutputFilePath, aggregatedResultsInFancyString);

            Console.WriteLine("Press ESC to exit...");
            while (Console.ReadKey().Key == ConsoleKey.Escape) { }
        }
        
        private static IEnumerable<AnalyzerExecutionTime> AggregatedResultsPerAnalyzer(IEnumerable<IEnumerable<AnalyzerExecutionTime>> analyzerExecutionTimesPerProject)
        {
            var aggregatedExecutionTimes = new Dictionary<string, double>();
            foreach (var executionTimesForPoject in analyzerExecutionTimesPerProject)
            {
                foreach (var singleExecutionTime in executionTimesForPoject)
                {
                    double currentTime;
                    var wasFound = aggregatedExecutionTimes.TryGetValue(singleExecutionTime.AnalyzerName, out currentTime);
                    var aggregatedExecutionTime = singleExecutionTime.ExecutionTime + (wasFound ? currentTime : 0);
                    aggregatedExecutionTimes[singleExecutionTime.AnalyzerName] = aggregatedExecutionTime;
                }
            }

            var result =  aggregatedExecutionTimes
                .Select(pair => new AnalyzerExecutionTime {AnalyzerName = pair.Key, ExecutionTime = pair.Value})
                .ToList();

            result.Sort((time1, time2) => time2.ExecutionTime.CompareTo(time1.ExecutionTime));

            return result;
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

        private static string ResultsInFancyString(IList<IEnumerable<AnalyzerExecutionTime>> analyzerExecutionTimesPerProject)
        {
            var result = new StringBuilder();
            for (var i = 0; i < analyzerExecutionTimesPerProject.Count; i++)
            {
                result.AppendLine($"Project {i}");
                foreach (var executionTime in analyzerExecutionTimesPerProject[i])
                {
                    result.AppendLine(executionTime.ToString());
                }
            }

            return result.ToString();
        }
    }
}
