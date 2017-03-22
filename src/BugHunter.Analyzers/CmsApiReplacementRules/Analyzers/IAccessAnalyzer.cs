using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.CmsApiReplacementRules.Analyzers
{
    public interface IAccessAnalyzer
    {
        bool IsForbiddenUsage(SyntaxNodeAnalysisContext context);
        void ReportDiagnostic(SyntaxNodeAnalysisContext context, DiagnosticDescriptor descriptor);
    }
}