using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers
{
    public class StringMethodsRuleBuilder
    {
        public static DiagnosticDescriptor CreateRuleForComparisonMethods(string diagnosticId)
            => new DiagnosticDescriptor(diagnosticId,
                title: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                category: nameof(AnalyzerCategories.StringAndCulture),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));

        public static DiagnosticDescriptor CreateRuleForManipulationMethods(string diagnosticId)
           => new DiagnosticDescriptor(diagnosticId,
            title: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            category: nameof(AnalyzerCategories.StringAndCulture),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
            helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));
    }
}