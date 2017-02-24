//// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
//// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Threading;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.MSBuild;

//namespace BugHunter.PerformanceTest
//{
//    /// <summary>
//    /// Prints out statistics about CMSSolution
//    /// </summary>
//    internal static class Program2
//    {
//        private static void Main(string[] args)
//        {
//            var cts = new CancellationTokenSource();
//            Console.CancelKeyPress +=
//                (sender, e) =>
//                {
//                    e.Cancel = true;
//                    cts.Cancel();
//                };

//            // Since Console apps do not have a SynchronizationContext, we're leveraging the built-in support
//            // in WPF to pump the messages via the Dispatcher.
//            // See the following for additional details:
//            //   http://blogs.msdn.com/b/pfxteam/archive/2012/01/21/10259307.aspx
//            //   https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1362
//            var previousContext = SynchronizationContext.Current;
//            try
//            {
//                var context = new DispatcherSynchronizationContext();
//                SynchronizationContext.SetSynchronizationContext(context);

//                var dispatcherFrame = new DispatcherFrame();
//                var mainTask = MainAsync(args, cts.Token);
//                mainTask.ContinueWith(task => dispatcherFrame.Continue = false, cts.Token);

//                Dispatcher.PushFrame(dispatcherFrame);
//                mainTask.GetAwaiter().GetResult();
//            }
//            finally
//            {
//                SynchronizationContext.SetSynchronizationContext(previousContext);
//            }
//        }

//        private static async Task MainAsync(string[] args, CancellationToken cancellationToken)
//        {
//            var stopwatch = Stopwatch.StartNew();

//            var workspace = MSBuildWorkspace.Create();
//            var solutionPath = @"C:\TFS\CMS\MAIN\CMSSolution\CMSSolution.sln";
//            //var solutionPath = @"C:\Users\zuzanad\code\thesis\thesis-sample-test-project\SampleProject\SampleProject.sln";
//            var solution = await workspace.OpenSolutionAsync(solutionPath, cancellationToken).ConfigureAwait(false);

//            Console.WriteLine($@"Loaded solution in {stopwatch.Elapsed:mm\:ss\.ff}");
//            stopwatch.Restart();

//            var csharpProjects = solution.Projects.Where(i => i.Language == LanguageNames.CSharp && !IsProjectExcluded(i)).ToList();

//            Console.WriteLine("Number of projects: \t\t\t{0,12:N}", csharpProjects.Count);
//            Console.WriteLine("Number of documents:\t\t\t{0,12:N}", csharpProjects.Sum(x => x.DocumentIds.Count));

//            var statistics = await StatisticsHelper.GetAnalyzerStatisticsAsync(csharpProjects, cancellationToken, IsProjectExcluded).ConfigureAwait(true);

//            StatisticsHelper.PrintStatistics(statistics);

//            Console.WriteLine($@"The end. Stats computed in {stopwatch.Elapsed:mm\:ss\.ff} Press ESC to continue...");
//            while (Console.ReadKey().Key != ConsoleKey.Escape) ;
//        }

//        private static bool IsProjectExcluded(Project project)
//        {
//            var thirdPartyProjects = new[]
//            {
//                "Contrib.WordNet.SynExpand",
//                "FiftyOne",
//                "ITHitWebDAVServer",
//                "Lucene.Net.v3",
//                "PDFClown",
//                "QRCodeLib",
//            };

//            Console.WriteLine(project.FilePath);
//            return project.FilePath.Contains("Tests") || thirdPartyProjects.Any(thirdPartyProject => project.FilePath.Contains(thirdPartyProject));
//        }
//    }
//}
