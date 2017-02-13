using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers.DiagnosticDescriptionBuilders
{
    public static class BaseClassesInheritanceRuleBuilder
    {
        public static DiagnosticDescriptor GetRule(string diagnosticId, string typeOfFile, string suggestedBaseClass)
        {
            var rule = new DiagnosticDescriptor(diagnosticId,
                title: new LocalizableResourceString(nameof(Resources.BaseClasses_Title), Resources.ResourceManager, typeof(Resources), typeOfFile, suggestedBaseClass),
                messageFormat: new LocalizableResourceString(nameof(Resources.BaseClasses_MessageFormat), Resources.ResourceManager, typeof(Resources), "{0}", suggestedBaseClass),
                category: AnalyzerCategories.CmsApiReplacements,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(Resources.BaseClasses_Description), Resources.ResourceManager, typeof(Resources), typeOfFile, suggestedBaseClass));

            return rule;
        }
    }
}