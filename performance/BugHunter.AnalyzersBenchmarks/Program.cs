using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BugHunter.AnalyzersBenchmarks.Benchmarks.BaseClasses;
using BugHunter.AnalyzersBenchmarks.Benchmarks.CallbacksBinding;
using BugHunter.AnalyzersBenchmarks.Benchmarks.SystemIo;
using BugHunter.AnalyzersBenchmarks.Configuration;

namespace BugHunter.AnalyzersBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ProductionConfig();

            //RunBenchmarks(congig);
            //RunSystemIOAnalysis();

            //RunBenchmarksForPage(config);
            //RunPageBaseAnalysis();


            var totalTime = BenchmarkRunner.Run<SingleVsDoubleCallbackBenchmark>(config).TotalTime;
            Console.WriteLine($@"Benchmarks executed. Total time:  {totalTime:mm\:ss\.ff}");
            //var analyzerV1SingleCallback = new SingleVsDoubleCallbackBenchmark().AnalyzerV1_SingleCallback();
            //Console.WriteLine("Done... " + analyzerV1SingleCallback);

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void RunBenchmarks(ManualConfig config)
        {
            Console.WriteLine("Benchmarks execution started. This is gonna take a while, go make yourself a coffee...");

            var totalTime1 = BenchmarkRunner.Run<SourcesWithNoErrorsBenchmark>(config).TotalTime;
            var totalTime2 = BenchmarkRunner.Run<SourcesWithErrorsBenchmark>(config).TotalTime;
            var totalTime3 = BenchmarkRunner.Run<SourcesFromConsoleAppBenchmark>(config).TotalTime;

            var totalTime4 = BenchmarkRunner.Run<PageBaseBenchmark>(config).TotalTime;

            Console.WriteLine($@"Benchmarks executed. Total times:");
            Console.WriteLine($@"SourcesWithNoErrorsBenchmark: {totalTime1:mm\:ss\.ff}");
            Console.WriteLine($@"SourcesWithErrorsBenchmark: {totalTime2:mm\:ss\.ff}");
            Console.WriteLine($@"SourcesFromConsoleAppBenchmark: {totalTime3:mm\:ss\.ff}");
            Console.WriteLine($@"PageBaseBenchmark: {totalTime4:mm\:ss\.ff}");
        }

        private static void RunBenchmarksForPage(ManualConfig config)
        {
            Console.WriteLine("Benchmarks execution started. This is gonna take a while, go make yourself a coffee...");

            var totalTime1 = BenchmarkRunner.Run<PageBaseBenchmark>(config).TotalTime;
            var totalTime2 = BenchmarkRunner.Run<PageBaseBenchmarkNoDiagnosticCode>(config).TotalTime;

            Console.WriteLine($@"Benchmarks executed. Total times:");
            Console.WriteLine($@"PageBaseBenchmark: {totalTime1:mm\:ss\.ff}");
            Console.WriteLine($@"PageBaseBenchmarkNoDiagnosticCode: {totalTime2:mm\:ss\.ff}");
        }

        private static void RunSystemIOAnalysis()
        {
            var numberOfDiagnosticsRaised = new SourcesWithErrorsBenchmark().AnalyzerV5_CompilationStartSyntaxNodeAndCompilationEnd();
            Console.WriteLine($@"Analysis finished. Total of {numberOfDiagnosticsRaised} diagnostics found.");
        }

        private static void RunPageBaseAnalysis()
        {
            var pageBaseBenchmark = new PageBaseBenchmark();
            var numberOfDiagnosticsRaised1 = pageBaseBenchmark.AnalyzerV1_SyntxNodeRegistered();
            var numberOfDiagnosticsRaised2 = pageBaseBenchmark.AnalyzerV2_NamedTypeSymbolRegistered();
            Console.WriteLine($@"Analysis finished. Total of {numberOfDiagnosticsRaised1} and {numberOfDiagnosticsRaised2} diagnostics found.");
        }
    }
}
