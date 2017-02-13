using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers
{
    public static class ApiReplacementRuleBuilder
    {
        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage)
        {
            var rule = new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessageBuilder.GetTitle(forbiddenUsage),
                messageFormat: ApiReplacementsMessageBuilder.GetMessageFormat(),
                category: AnalyzerCategories.CmsApiReplacements,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessageBuilder.GetDescription(forbiddenUsage));

            return rule;
        }

        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage, string recommendedUsage)
        {
            var rule = new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessageBuilder.GetTitle(forbiddenUsage, recommendedUsage),
                messageFormat: ApiReplacementsMessageBuilder.GetMessageFormat(recommendedUsage),
                category: AnalyzerCategories.CmsApiReplacements,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessageBuilder.GetDescription(forbiddenUsage, recommendedUsage));

            return rule;
        }
    }
}