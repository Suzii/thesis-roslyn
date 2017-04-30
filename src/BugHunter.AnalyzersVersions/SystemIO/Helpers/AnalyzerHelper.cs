using BugHunter.Core;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;

namespace BugHunter.AnalyzersVersions.SystemIO.Helpers
{
    public static class AnalyzerHelper
    {
        public static readonly string[] WhiteListedTypeNames =
        {
            "System.IO.IOException",
            "System.IO.Stream",
            "System.IO.SeekOrigin"
        };

        public static DiagnosticDescriptor GetRule(string diagnosticId) => new DiagnosticDescriptor(diagnosticId,
                title: "Do not use System.IO",
                messageFormat: "'{0}' should not use 'System.IO' directly. Use equivalent method from namespace 'CMS.IO'.",
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "'System.IO' namespace should not be used directly. Use equivalent method from namespace 'CMS.IO'.");
    }
}