using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.DiagnosticDescriptors;
using Microsoft.CodeAnalysis;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers
{
    /// <summary>
    /// Helper class for constructing <see cref="DiagnosticDescriptor"/> for String and Culture Analyzers
    /// </summary>
    public class StringMethodsRuleBuilder
    {
        /// <summary>
        /// Constructs a <see cref="DiagnosticDescriptor"/> for String comparison methods Analyzer.
        /// </summary>
        /// <remarks>
        /// Analyzer will have provided diagnostic ID, StringAndCulture category,
        /// Warning severity, HelperLinkUri, and will be enabled by default
        /// </remarks>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <returns>Diagnostic descriptor for String comparison analyzer</returns>
        public static DiagnosticDescriptor CreateRuleForComparisonMethods(string diagnosticId)
            => new DiagnosticDescriptor(
                diagnosticId,
                title: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                category: nameof(AnalyzerCategories.StringAndCulture),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));

        /// <summary>
        /// Constructs a <see cref="DiagnosticDescriptor"/> for String manipulation methods Analyzer.
        /// </summary>
        /// <remarks>
        /// Analyzer will have provided diagnostic ID, StringAndCulture category,
        /// Warning severity, HelperLinkUri, and will be enabled by default
        /// </remarks>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <returns>Diagnostic descriptor for String manipulation analyzer</returns>
        public static DiagnosticDescriptor CreateRuleForManipulationMethods(string diagnosticId)
           => new DiagnosticDescriptor(
               diagnosticId,
                title: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                category: nameof(AnalyzerCategories.StringAndCulture),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(StringMethodsResources.StringManipulationMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));
    }
}