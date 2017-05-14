// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ReportAnalyzerTimes.Models;

namespace ReportAnalyzerTimes.Parser
{
    /// <summary>
    /// Reads the MSBuild output from file name passed as parameter "/in=XY",
    /// parses the contnets of ReportAnalyzer sections,
    /// groups per projects ReportAnalyzer results into one summed representation,
    /// outputs the aggregated results into file passed as "/out=XY"
    /// </summary>
    internal class Program
    {
        private static int Main(string[] args)
        {
            var inputFilePath = args.FirstOrDefault(arg => arg.StartsWith("/in="))?.Substring(4);
            var aggregatedOutputFilePath = args.FirstOrDefault(arg => arg.StartsWith("/out="))?.Substring(5);

            if (string.IsNullOrEmpty(inputFilePath) || string.IsNullOrEmpty(aggregatedOutputFilePath))
            {
                PrintHelp();
                return -1;
            }

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"File {inputFilePath} does not exist.");
                PrintHelp();
                return -1;
            }

            Console.WriteLine($"Reading contents of {inputFilePath}");
            var inputFileContent = string.Join(Environment.NewLine, File.ReadLines(inputFilePath));

            var executionTimesPerProject = new MsBuildLogParser().GetAnalyzerExecutionTimesForProjects(inputFileContent);

            var aggregatedResultsPerAnalyzer = new AnalyzerExecutionTimesAggregator().GetAggregatedResultsPerAnalyzer(executionTimesPerProject);

            var aggregatedResultsInFancyString = aggregatedResultsPerAnalyzer.Select(time => time.ToString());
            File.WriteAllLines(aggregatedOutputFilePath, aggregatedResultsInFancyString);
            Console.WriteLine($"Execution times aggregated and written to {aggregatedOutputFilePath}");

            return 0;
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

        private static void PrintHelp()
        {
            Console.WriteLine(@"Usage:");
            Console.WriteLine(@"ReportAnalyzerTimes.Parse.exe /in=C:\tmp\ms-build-output.txt /out=C:\tmp\analyzers-execution-times-aggregated.txt");
        }
    }
}
