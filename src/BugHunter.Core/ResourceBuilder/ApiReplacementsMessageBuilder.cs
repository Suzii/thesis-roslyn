using Microsoft.CodeAnalysis;

namespace BugHunter.Core.ResourceBuilder
{
    public static class ApiReplacementsMessageBuilder
    {
        public static LocalizableResourceString GetTitle(string forbiddenUsage, string suggestedUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Title), Resources.ResourceManager, typeof(Resources), forbiddenUsage, suggestedUsage);
        }

        public static LocalizableResourceString GetTitle(string forbiddenUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Title_NoSuggestion), Resources.ResourceManager, typeof(Resources), forbiddenUsage);
        }
        
        public static LocalizableResourceString GetMessageFormat(string suggestedUsage)
        {
            // quite a hack, so that we can format string for a second time with the actual usage
            var placeholderForActualUsage = "{0}";
            var message = new LocalizableResourceString(nameof(Resources.ApiReplacements_MessageFormat), Resources.ResourceManager, typeof(Resources), placeholderForActualUsage, suggestedUsage);
            return message;
        }

        public static LocalizableResourceString GetMessageFormat()
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_MessageFormat_NoSuggestion), Resources.ResourceManager, typeof(Resources));
        }

        public static LocalizableResourceString GetDescription(string forbiddenUsage, string suggestedUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Description), Resources.ResourceManager, typeof(Resources), forbiddenUsage, suggestedUsage);
        }

        public static LocalizableResourceString GetDescription(string forbiddenUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Description_NoSuggestion), Resources.ResourceManager, typeof(Resources), forbiddenUsage);
        }
    }
}