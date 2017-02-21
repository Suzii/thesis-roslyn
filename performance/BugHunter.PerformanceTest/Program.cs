// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using BugHunter.Analyzers.CmsApiReplacementRules.Analyzers;
using BugHunter.PerformanceTest.Helpers;
using BugHunter.PerformanceTest.Models;
using BugHunter.Web.Analyzers.CmsBaseClassesRules.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using File = System.IO.File;
using Path = System.IO.Path;

namespace BugHunter.PerformanceTest
{
    /// <summary>
    /// StyleCopTester is a tool that will analyze a solution, find diagnostics in it and will print out the number of
    /// diagnostics it could find. This is useful to easily test performance without having the overhead of visual
    /// studio running.
    /// </summary>
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress +=
                (sender, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

            // Since Console apps do not have a SynchronizationContext, we're leveraging the built-in support
            // in WPF to pump the messages via the Dispatcher.
            // See the following for additional details:
            //   http://blogs.msdn.com/b/pfxteam/archive/2012/01/21/10259307.aspx
            //   https://github.com/DotNetAnalyzers/StyleCopAnalyzers/pull/1362
            var previousContext = SynchronizationContext.Current;
            try
            {
                var context = new DispatcherSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(context);

                var dispatcherFrame = new DispatcherFrame();
                var mainTask = MainAsync(args, cts.Token);
                mainTask.ContinueWith(task => dispatcherFrame.Continue = false, cts.Token);

                Dispatcher.PushFrame(dispatcherFrame);
                mainTask.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        private static async Task MainAsync(string[] args, CancellationToken cancellationToken)
        {
            // A valid call must have at least one parameter (a solution file). Optionally it can include /all or /id:SAXXXX.
            if (args.Length < 1)
            {
                PrintHelp();
                return;
            }

            var applyChanges = args.Contains("/apply");
            if (applyChanges)
            {
                if (!args.Contains("/fixall"))
                {
                    Console.Error.WriteLine("Error: /apply can only be used with /fixall");
                    return;
                }
            }

            var stopwatch = Stopwatch.StartNew();
            var analyzers = GetAllAnalyzers();

            analyzers = FilterAnalyzers(analyzers, args).ToImmutableArray();

            if (analyzers.Length == 0)
            {
                PrintHelp();
                return;
            }

            var workspace = MSBuildWorkspace.Create();
            var solutionPath = args.SingleOrDefault(i => !i.StartsWith("/", StringComparison.Ordinal));
            var solution = await workspace.OpenSolutionAsync(solutionPath, cancellationToken).ConfigureAwait(false);

            Console.WriteLine($@"Loaded solution in {stopwatch.Elapsed:mm\:ss\.ff}");

            if (args.Contains("/stats"))
            {
                var csharpProjects = solution.Projects.Where(i => i.Language == LanguageNames.CSharp).ToList();

                Console.WriteLine("Number of projects: \t\t{0,10:N}", csharpProjects.Count);
                Console.WriteLine("Number of documents:\t\t{0,10:N}", csharpProjects.Sum(x => x.DocumentIds.Count));

                var statistics = await StatisticsHelper.GetAnalyzerStatisticsAsync(csharpProjects, cancellationToken).ConfigureAwait(true);
                
                StatisticsHelper.PrintStatistics(statistics);
            }

            var force = args.Contains("/force");

            // TODO  execute following code multiple times and create statistics
            stopwatch.Restart();
            var diagnostics = await GetAnalyzerDiagnosticsAsync(solution, solutionPath, analyzers, force, cancellationToken).ConfigureAwait(true);
            var allDiagnostics = diagnostics.SelectMany(i => i.Value).ToImmutableArray();

            Console.WriteLine($@"Found {allDiagnostics.Length} diagnostics in {stopwatch.Elapsed:mm\:ss\.ff}");

            foreach (var group in allDiagnostics.GroupBy(i => i.Id).OrderBy(i => i.Key, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"  {@group.Key}: {@group.Count()} instances");

                // Print out analyzer diagnostics like AD0001 for analyzer exceptions
                if (@group.Key.StartsWith("AD", StringComparison.Ordinal))
                {
                    foreach (var item in @group)
                    {
                        Console.WriteLine(item);
                    }
                }
            }

            var logArgument = args.FirstOrDefault(x => x.StartsWith("/log:"));
            if (logArgument != null)
            {
                var fileName = logArgument.Substring(logArgument.IndexOf(':') + 1);
                WriteDiagnosticResults(diagnostics.SelectMany(i => i.Value.Select(j => Tuple.Create(i.Key, j))).ToImmutableArray(), fileName);
            }

            if (args.Contains("/codefixes"))
            {
                await TestCodeFixesAsync(stopwatch, solution, allDiagnostics, cancellationToken).ConfigureAwait(true);
            }

            if (args.Contains("/fixall"))
            {
                await TestFixAllAsync(stopwatch, solution, diagnostics, applyChanges, cancellationToken).ConfigureAwait(true);
            }
        }

        private static void WriteDiagnosticResults(ImmutableArray<Tuple<ProjectId, Diagnostic>> diagnostics, string fileName)
        {
            var orderedDiagnostics =
                diagnostics
                .OrderBy(i => i.Item2.Id)
                .ThenBy(i => i.Item2.Location.SourceTree?.FilePath, StringComparer.OrdinalIgnoreCase)
                .ThenBy(i => i.Item2.Location.SourceSpan.Start)
                .ThenBy(i => i.Item2.Location.SourceSpan.End);

            var uniqueLines = new HashSet<string>();
            var completeOutput = new StringBuilder();
            var uniqueOutput = new StringBuilder();
            foreach (var diagnostic in orderedDiagnostics)
            {
                var message = diagnostic.Item2.ToString();
                string uniqueMessage = $"{diagnostic.Item1}: {diagnostic.Item2}";
                completeOutput.AppendLine(message);
                if (uniqueLines.Add(uniqueMessage))
                {
                    uniqueOutput.AppendLine(message);
                }
            }

            var directoryName = Path.GetDirectoryName(fileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = Path.Combine(directoryName, $"{fileNameWithoutExtension}-Unique{extension}");

            File.WriteAllText(fileName, completeOutput.ToString(), Encoding.UTF8);
            File.WriteAllText(uniqueFileName, uniqueOutput.ToString(), Encoding.UTF8);
        }

        private static async Task TestFixAllAsync(Stopwatch stopwatch, Solution solution, ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>> diagnostics, bool applyChanges, CancellationToken cancellationToken)
        {
            Console.WriteLine("Calculating fixes");

            var codeFixers = GetAllCodeFixers().SelectMany(x => x.Value).Distinct();

            var equivalenceGroups = new List<CodeFixEquivalenceGroup>();

            foreach (var codeFixer in codeFixers)
            {
                equivalenceGroups.AddRange(await CodeFixEquivalenceGroup.CreateAsync(codeFixer, diagnostics, solution, cancellationToken).ConfigureAwait(true));
            }

            Console.WriteLine($"Found {equivalenceGroups.Count} equivalence groups.");
            if (applyChanges && equivalenceGroups.Count > 1)
            {
                Console.Error.WriteLine("/apply can only be used with a single equivalence group.");
                return;
            }

            Console.WriteLine("Calculating changes");

            foreach (var fix in equivalenceGroups)
            {
                try
                {
                    stopwatch.Restart();
                    Console.WriteLine($"Calculating fix for {fix.CodeFixEquivalenceKey} using {fix.FixAllProvider} for {fix.NumberOfDiagnostics} instances.");
                    var operations = await fix.GetOperationsAsync(cancellationToken).ConfigureAwait(true);
                    if (applyChanges)
                    {
                        var applyOperations = operations.OfType<ApplyChangesOperation>().ToList();
                        if (applyOperations.Count > 1)
                        {
                            Console.Error.WriteLine("/apply can only apply a single code action operation.");
                        }
                        else if (applyOperations.Count == 0)
                        {
                            Console.WriteLine("No changes were found to apply.");
                        }
                        else
                        {
                            applyOperations[0].Apply(solution.Workspace, cancellationToken);
                        }
                    }

                    WriteLine($"Calculating changes completed in {stopwatch.ElapsedMilliseconds}ms. This is {fix.NumberOfDiagnostics / stopwatch.Elapsed.TotalSeconds:0.000} instances/second.", ConsoleColor.Yellow);
                }
                catch (Exception ex)
                {
                    // Report thrown exceptions
                    WriteLine($"The fix '{fix.CodeFixEquivalenceKey}' threw an exception after {stopwatch.ElapsedMilliseconds}ms:", ConsoleColor.Yellow);
                    WriteLine(ex.ToString(), ConsoleColor.Yellow);
                }
            }
        }

        private static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static async Task TestCodeFixesAsync(Stopwatch stopwatch, Solution solution, ImmutableArray<Diagnostic> diagnostics, CancellationToken cancellationToken)
        {
            Console.WriteLine("Calculating fixes");

            var fixes = new List<CodeAction>();

            var codeFixers = GetAllCodeFixers();

            foreach (var item in diagnostics)
            {
                foreach (var codeFixer in codeFixers.GetValueOrDefault(item.Id, ImmutableList.Create<CodeFixProvider>()))
                {
                    fixes.AddRange(await GetFixesAsync(solution, codeFixer, item, cancellationToken).ConfigureAwait(false));
                }
            }

            Console.WriteLine($"Found {fixes.Count} potential code fixes");

            Console.WriteLine("Calculating changes");

            stopwatch.Restart();

            var lockObject = new object();

            Parallel.ForEach(fixes, fix =>
            {
                try
                {
                    fix.GetOperationsAsync(cancellationToken).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // Report thrown exceptions
                    lock (lockObject)
                    {
                        WriteLine($"The fix '{fix.Title}' threw an exception:", ConsoleColor.Yellow);
                        WriteLine(ex.ToString(), ConsoleColor.Red);
                    }
                }
            });

            Console.WriteLine($"Calculating changes completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static async Task<IEnumerable<CodeAction>> GetFixesAsync(Solution solution, CodeFixProvider codeFixProvider, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var codeActions = new List<CodeAction>();

            await codeFixProvider.RegisterCodeFixesAsync(new CodeFixContext(solution.GetDocument(diagnostic.Location.SourceTree), diagnostic, (a, d) => codeActions.Add(a), cancellationToken)).ConfigureAwait(false);

            return codeActions;
        }

        private static IEnumerable<DiagnosticAnalyzer> FilterAnalyzers(IEnumerable<DiagnosticAnalyzer> analyzers, string[] args)
        {
            var useAll = args.Contains("/all");

            var ids = new HashSet<string>(args.Where(y => y.StartsWith("/id:", StringComparison.Ordinal)).Select(y => y.Substring(4)));

            foreach (var analyzer in analyzers)
            {
                if (useAll)
                {
                    yield return analyzer;
                }
                else if (ids.Count == 0)
                {
                    if (analyzer.SupportedDiagnostics.Any(i => i.IsEnabledByDefault))
                    {
                        yield return analyzer;
                    }

                    continue;
                }
                else if (analyzer.SupportedDiagnostics.Any(y => ids.Contains(y.Id)))
                {
                    yield return analyzer;
                }
            }
        }

        private static ImmutableArray<DiagnosticAnalyzer> GetAllAnalyzers()
        {
            var analyzersAssembly = typeof(ClientScriptMethodsAnalyzer).Assembly;
            var webAnalyzersAssembly = typeof(WebPartBaseAnalyzer).Assembly;

            var diagnosticAnalyzerType = typeof(DiagnosticAnalyzer);

            var analyzers = ImmutableArray.CreateBuilder<DiagnosticAnalyzer>();

            foreach (var type in analyzersAssembly.GetTypes().Union(webAnalyzersAssembly.GetTypes()))
            {
                if (type.IsSubclassOf(diagnosticAnalyzerType) && !type.IsAbstract)
                {
                    analyzers.Add((DiagnosticAnalyzer)Activator.CreateInstance(type));
                }
            }

            return analyzers.ToImmutable();
        }

        private static ImmutableDictionary<string, ImmutableList<CodeFixProvider>> GetAllCodeFixers()
        {
            var analyzersAssembly = typeof(ClientScriptMethodsAnalyzer).Assembly;
            var webAnalyzersAssembly = typeof(WebPartBaseAnalyzer).Assembly;

            var codeFixProviderType = typeof(CodeFixProvider);

            var providers = new Dictionary<string, ImmutableList<CodeFixProvider>>();

            foreach (var type in analyzersAssembly.GetTypes().Union(webAnalyzersAssembly.GetTypes()))
            {
                if (!type.IsSubclassOf(codeFixProviderType) || type.IsAbstract)
                {
                    continue;
                }

                var codeFixProvider = (CodeFixProvider)Activator.CreateInstance(type);

                foreach (var diagnosticId in codeFixProvider.FixableDiagnosticIds)
                {
                    providers.AddToInnerList(diagnosticId, codeFixProvider);
                }
            }

            return providers.ToImmutableDictionary();
        }

        // TODO what is this good for?????
        private static ImmutableDictionary<FixAllProvider, ImmutableHashSet<string>> GetAllFixAllProviders(IEnumerable<CodeFixProvider> providers)
        {
            var fixAllProviders = new Dictionary<FixAllProvider, ImmutableHashSet<string>>();

            foreach (var provider in providers)
            {
                var fixAllProvider = provider.GetFixAllProvider();
                var supportedDiagnosticIds = fixAllProvider.GetSupportedFixAllDiagnosticIds(provider);
                foreach (var id in supportedDiagnosticIds)
                {
                    fixAllProviders.AddToInnerSet(fixAllProvider, id);
                }
            }

            return fixAllProviders.ToImmutableDictionary();
        }

        private static async Task<ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>>> GetAnalyzerDiagnosticsAsync(Solution solution, string solutionPath, ImmutableArray<DiagnosticAnalyzer> analyzers, bool force, CancellationToken cancellationToken)
        {
            var projectDiagnosticTasks = new List<KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>>();

            // Make sure we analyze the projects in parallel
            foreach (var project in solution.Projects)
            {
                if (project.Language != LanguageNames.CSharp)
                {
                    continue;
                }

                projectDiagnosticTasks.Add(new KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>(project.Id, GetProjectAnalyzerDiagnosticsAsync(analyzers, project, force, cancellationToken)));
            }

            var projectDiagnosticBuilder = ImmutableDictionary.CreateBuilder<ProjectId, ImmutableArray<Diagnostic>>();
            foreach (var task in projectDiagnosticTasks)
            {
                projectDiagnosticBuilder.Add(task.Key, await task.Value.ConfigureAwait(false));
            }

            return projectDiagnosticBuilder.ToImmutable();
        }

        /// <summary>
        /// Returns a list of all analyzer diagnostics inside the specific project. This is an asynchronous operation.
        /// </summary>
        /// <param name="analyzers">The list of analyzers that should be used</param>
        /// <param name="project">The project that should be analyzed</param>
        /// <param name="force"><see langword="true"/> to force the analyzers to be enabled; otherwise,
        /// <see langword="false"/> to use the behavior configured for the specified <paramref name="project"/>.</param>
        /// <param name="cancellationToken">The cancellation token that the task will observe.</param>
        /// <returns>A list of diagnostics inside the project</returns>
        private static async Task<ImmutableArray<Diagnostic>> GetProjectAnalyzerDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, Project project, bool force, CancellationToken cancellationToken)
        {
            var supportedDiagnosticsSpecificOptions = new Dictionary<string, ReportDiagnostic>();
            if (force)
            {
                foreach (var analyzer in analyzers)
                {
                    foreach (var diagnostic in analyzer.SupportedDiagnostics)
                    {
                        // make sure the analyzers we are testing are enabled
                        supportedDiagnosticsSpecificOptions[diagnostic.Id] = ReportDiagnostic.Default;
                    }
                }
            }

            // Report exceptions during the analysis process as errors
            supportedDiagnosticsSpecificOptions.Add("AD0001", ReportDiagnostic.Error);

            // update the project compilation options
            // TODO check what this does and how can be disable CS checks
            var modifiedSpecificDiagnosticOptions = supportedDiagnosticsSpecificOptions.ToImmutableDictionary().SetItems(project.CompilationOptions.SpecificDiagnosticOptions);
            var modifiedCompilationOptions = project.CompilationOptions.WithSpecificDiagnosticOptions(modifiedSpecificDiagnosticOptions);
            var processedProject = project.WithCompilationOptions(modifiedCompilationOptions);

            var compilation = await processedProject.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
            // TODO add handler for onAnalyzerException
            var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers, new CompilationWithAnalyzersOptions(new AnalyzerOptions(ImmutableArray.Create<AdditionalText>()), null, true, false));

            var diagnostics = await FixAllContextHelper.GetAllDiagnosticsAsync(compilation, compilationWithAnalyzers, analyzers, project.Documents, true, cancellationToken).ConfigureAwait(false);
            return diagnostics;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("Usage: StyleCopTester [options] <Solution>");
            Console.WriteLine("Options:");
            Console.WriteLine("/all       Run all StyleCopAnalyzers analyzers, including ones that are disabled by default");
            Console.WriteLine("/stats     Display statistics of the solution");
            Console.WriteLine("/codefixes Test single code fixes");
            Console.WriteLine("/fixall    Test fix all providers");
            Console.WriteLine("/id:<id>   Enable analyzer with diagnostic ID < id > (when this is specified, only this analyzer is enabled)");
            Console.WriteLine("/apply     Write code fix changes back to disk");
            Console.WriteLine("/force     Force an analyzer to be enabled, regardless of the configured rule set(s) for the solution");
        }
    }
}
