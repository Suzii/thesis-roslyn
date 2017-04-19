using BugHunter.Core;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers
{
    internal class StringMethodsRuleBuilder
    {
        public static DiagnosticDescriptor CreateRuleForComparisonMethods(string diagnosticId)
            => new DiagnosticDescriptor(diagnosticId,
                title: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                category: nameof(AnalyzerCategories.StringAndCulture),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)));

        public static DiagnosticDescriptor CreateRuleForManipulationMethods(string diagnosticId)
           => new DiagnosticDescriptor(diagnosticId,
            title: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            category: nameof(AnalyzerCategories.StringAndCulture),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)));
    }
}