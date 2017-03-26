using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers.DiagnosticDescriptionBuilders
{
    public static class ApiReplacementRuleBuilder
    {
        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage)
            => new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessageBuilder.GetTitle(forbiddenUsage),
                messageFormat: ApiReplacementsMessageBuilder.GetMessageFormat(),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessageBuilder.GetDescription(forbiddenUsage));

        public static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage, string recommendedUsage)
           => new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessageBuilder.GetTitle(forbiddenUsage, recommendedUsage),
                messageFormat: ApiReplacementsMessageBuilder.GetMessageFormat(recommendedUsage),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessageBuilder.GetDescription(forbiddenUsage, recommendedUsage));
    }
}