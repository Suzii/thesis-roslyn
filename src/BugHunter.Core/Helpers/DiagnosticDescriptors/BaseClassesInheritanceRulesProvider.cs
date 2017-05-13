using BugHunter.Core.Constants;
using BugHunter.Core.Helpers.ResourceMessages;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers.DiagnosticDescriptors
{
    public static class BaseClassesInheritanceRulesProvider
    {
        /// <summary>
        /// Constructs a <see cref="DiagnosticDescriptor"/> for CMS Base Classes Analyzers.
        /// </summary>
        /// <remarks>
        /// Analyzer will have provided diagnostic ID, CmsBaseClass category,
        /// Warning severity, HelperLinkUri, and will be enabled by default
        /// </remarks>
        /// <param name="diagnosticId">Diagnostic ID of the rule</param>
        /// <param name="typeOfFile">Message argument with type of file that the rule applies to</param>
        /// <param name="suggestedBaseClass">Message argument with suggested base class to be used</param>
        /// <returns>Diagnostic descriptor for Base Class Analyzer</returns>
        public static DiagnosticDescriptor GetRule(string diagnosticId, string typeOfFile, string suggestedBaseClass)
        {
            var rule = new DiagnosticDescriptor(diagnosticId,
                title: new LocalizableResourceString(nameof(Resources.BaseClasses_Title), Resources.ResourceManager, typeof(Resources), typeOfFile, suggestedBaseClass),
                messageFormat: new LocalizableResourceString(nameof(Resources.BaseClasses_MessageFormat), Resources.ResourceManager, typeof(Resources), "{0}", suggestedBaseClass),
                category: nameof(AnalyzerCategories.CmsApiReplacements),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.BaseClasses_Description), Resources.ResourceManager, typeof(Resources), typeOfFile, suggestedBaseClass),
                helpLinkUri: HelpLinkUriProvider.GetHelpLink(diagnosticId));

            return rule;
        }
    }
}