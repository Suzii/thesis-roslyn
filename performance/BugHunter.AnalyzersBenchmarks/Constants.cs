using System;
using System.IO;

namespace BugHunter.AnalyzersBenchmarks
{
    public static class Constants
    {
        public static readonly string PathToSampleProject = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\thesis-sample-test-project\SampleProject"));

        //@"C:\Users\zuzanad\code\thesis\thesis-sample-test-project\SampleProject";
        //@"D:\code\thesis\thesis-sample-test-project\SampleProject";
    }
}