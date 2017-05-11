using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Analyzers
{
    /// <summary>
    /// An interface for analysis helpers analyzing <see cref="SyntaxNodeAnalysisContext"/> and raising diagnostics
    /// </summary>
    public interface ISyntaxNodeAnalyzer
    {
        /// <summary>
        /// Runs the analysis for current <paramref name="context"/> and raises <see cref="Microsoft.CodeAnalysis.Diagnostic"/> if usage is qualified as forbidden
        /// </summary>
        /// <param name="context">Context to perform analysis on</param>
        void Run(SyntaxNodeAnalysisContext context);
    }
}