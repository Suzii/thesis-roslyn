using BugHunter.TestUtils.Helpers;
using BugHunter.TestUtils.Services;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersBenchmarks.Benchmarks
{
    public class AnalysisRunner
    {
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
    }
}