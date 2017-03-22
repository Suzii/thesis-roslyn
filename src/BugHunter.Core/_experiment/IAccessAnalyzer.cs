using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    public interface IAccessAnalyzer
    {
        bool IsForbiddenUsage(SyntaxNodeAnalysisContext context);
        void ReportDiagnostic(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor);
    }
}