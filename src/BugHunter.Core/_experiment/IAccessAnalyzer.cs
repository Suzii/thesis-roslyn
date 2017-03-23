using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    public interface IAccessAnalyzer
    {
        void Run(SyntaxNodeAnalysisContext context);
    }
}