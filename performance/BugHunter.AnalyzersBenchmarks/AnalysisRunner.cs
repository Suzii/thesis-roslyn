using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks
{
    public class AnalysisRunner
    {
        public static Task<int> RunAnalysisAsync(Solution solution, params DiagnosticAnalyzer[] analyzers)
        {
            if (analyzers.Length == 0)
            {
                return Task.FromResult(0);
            }

            return RunAnalyzers(solution, analyzers.ToImmutableArray());
        }

        public static Task<int> RunAnalysisAsync(string[] sources, MetadataReference[] additionalReferences, FakeFileInfo fakeFileInfo = null, params DiagnosticAnalyzer[] analyzers)
        {
            var solution = ProjectCompilation.CreateSolutionWithSingleProject(sources, additionalReferences, fakeFileInfo);

            return RunAnalysisAsync(solution, analyzers);
        }

        public static int RunAnalysis(string[] sources, MetadataReference[] additionalReferences, FakeFileInfo fakeFileInfo = null, params DiagnosticAnalyzer[] analyzers)
        {
            var documents = ProjectCompilation.GetDocuments(sources, additionalReferences, fakeFileInfo);
            if (analyzers.Length == 0)
            {
                return 0;
            }

            return AnalyzerExecution.GetSortedDiagnosticsFromDocuments(analyzers, documents).Length;
        }

        public static int RunAnalysis(string source, MetadataReference[] additionalReferences, FakeFileInfo fakeFileInfo = null, params DiagnosticAnalyzer[] analyzers)
        {
            return RunAnalysis(new[] { source }, additionalReferences, fakeFileInfo, analyzers);
        }

        public static async Task<int> RunAnalyzers(Solution solution, ImmutableArray<DiagnosticAnalyzer> analyzers)
        {
            var diagnosticForProjects = await GetAnalyzerDiagnosticsAsync(solution, analyzers, CancellationToken.None).ConfigureAwait(true);

            return diagnosticForProjects.Select(project => project.Value).Sum(diagnostics => diagnostics.Length);
        }

        private static async Task<ImmutableDictionary<ProjectId, ImmutableArray<Diagnostic>>> GetAnalyzerDiagnosticsAsync(Solution solution, ImmutableArray<DiagnosticAnalyzer> analyzers, CancellationToken cancellationToken)
        {
            var projectDiagnosticTasks = new List<KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>>();

            // Make sure we analyze the projects in parallel
            foreach (var project in solution.Projects)
            {
                if (project.Language != LanguageNames.CSharp)
                {
                    continue;
                }

                projectDiagnosticTasks.Add(new KeyValuePair<ProjectId, Task<ImmutableArray<Diagnostic>>>(project.Id, GetProjectAnalyzerDiagnosticsAsync(analyzers, project, cancellationToken)));
            }

            var projectDiagnosticBuilder = ImmutableDictionary.CreateBuilder<ProjectId, ImmutableArray<Diagnostic>>();
            foreach (var task in projectDiagnosticTasks)
            {
                projectDiagnosticBuilder.Add(task.Key, await task.Value.ConfigureAwait(false));
            }

            return projectDiagnosticBuilder.ToImmutable();
        }

        private static async Task<ImmutableArray<Diagnostic>> GetProjectAnalyzerDiagnosticsAsync(ImmutableArray<DiagnosticAnalyzer> analyzers, Project project, CancellationToken cancellationToken)
        {
            var supportedDiagnosticsSpecificOptions = new Dictionary<string, ReportDiagnostic>();
            
            foreach (var analyzer in analyzers)
            {
                foreach (var diagnostic in analyzer.SupportedDiagnostics)
                {
                    // make sure the analyzers we are testing are enabled
                    supportedDiagnosticsSpecificOptions[diagnostic.Id] = ReportDiagnostic.Default;
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

            var diagnostics = await GetAllDiagnosticsAsync(compilationWithAnalyzers, cancellationToken).ConfigureAwait(false);
            return diagnostics;
        }

        private static async Task<ImmutableArray<Diagnostic>> GetAllDiagnosticsAsync(CompilationWithAnalyzers compilationWithAnalyzers, CancellationToken cancellationToken)
        {
            return await compilationWithAnalyzers.GetAllDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}