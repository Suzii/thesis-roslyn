using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BugHunter.AnalyzersBenchmarks.Benchmarks;

namespace BugHunter.AnalyzersBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SystemIoAnalyzerBenchmark>(new ManualConfig() {KeepBenchmarkFiles = true});
            
            Console.WriteLine("Benchmarks executed. Press Enter to exit...");
            Console.ReadLine();
        }
    }
}