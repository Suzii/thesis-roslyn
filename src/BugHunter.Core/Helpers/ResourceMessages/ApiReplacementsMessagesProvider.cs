// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers.ResourceMessages
{
    /// <summary>
    /// Helper class for constructing messages for API Replacement Analyzers' diagnostics
    /// </summary>
    internal static class ApiReplacementsMessagesProvider
    {
        /// <summary>
        /// Get title for <see cref="DiagnosticDescriptor"/> of API Replacement Analyzers with substituted message arguments
        /// </summary>
        /// <param name="forbiddenUsage">Forbidden usage</param>
        /// <param name="suggestedUsage">Suggested usage</param>
        /// <returns>Title of diagnostic descriptor with substituted arguments</returns>
        public static LocalizableResourceString GetTitle(string forbiddenUsage, string suggestedUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Title), Resources.ResourceManager, typeof(Resources), forbiddenUsage, suggestedUsage);
        }

        /// <summary>
        /// Get title for <see cref="DiagnosticDescriptor"/> of API Replacement Analyzers without specific no suggestion
        /// </summary>
        /// <param name="forbiddenUsage">Forbidden usage</param>
        /// <returns>Title of diagnostic descriptor with substituted argument</returns>
        public static LocalizableResourceString GetTitle(string forbiddenUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Title_NoSuggestion), Resources.ResourceManager, typeof(Resources), forbiddenUsage);
        }

        /// <summary>
        /// Get message format for <see cref="DiagnosticDescriptor"/> of API Replacement Analyzers with substituted suggested usage and placeholder for actual usage
        /// </summary>
        /// <param name="suggestedUsage">Suggested usage</param>
        /// <returns>Message format of diagnostic descriptor with suggested usage and placeholder for actual usage</returns>
        public static LocalizableResourceString GetMessageFormat(string suggestedUsage)
        {
            // quite a hack, so that we can format string for a second time with the actual usage
            var placeholderForActualUsage = "{0}";
            var message = new LocalizableResourceString(nameof(Resources.ApiReplacements_MessageFormat), Resources.ResourceManager, typeof(Resources), placeholderForActualUsage, suggestedUsage);
            return message;
        }

        /// <summary>
        /// Get message format for <see cref="DiagnosticDescriptor"/> of API Replacement Analyzers with placeholder for actual usage
        /// </summary>
        /// <returns>Message format of diagnostic descriptor with placeholder for actual usage</returns>
        public static LocalizableResourceString GetMessageFormat()
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_MessageFormat_NoSuggestion), Resources.ResourceManager, typeof(Resources));
        }

        /// <summary>
        /// Get description for <see cref="DiagnosticDescriptor"/> of API Replacement Analyzers with substituted message arguments
        /// </summary>
        /// <param name="forbiddenUsage">Forbidden usage</param>
        /// <param name="suggestedUsage">Suggested usage</param>
        /// <returns>Description of diagnostic descriptor with substituted arguments</returns>
        public static LocalizableResourceString GetDescription(string forbiddenUsage, string suggestedUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Description), Resources.ResourceManager, typeof(Resources), forbiddenUsage, suggestedUsage);
        }

        /// <summary>
        /// Get description for <see cref="DiagnosticDescriptor"/> of API Replacement Analyzers with substituted message argument
        /// </summary>
        /// <param name="forbiddenUsage">Forbidden usage</param>
        /// <returns>Description of diagnostic descriptor with substituted argument</returns>
        public static LocalizableResourceString GetDescription(string forbiddenUsage)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_Description_NoSuggestion), Resources.ResourceManager, typeof(Resources), forbiddenUsage);
        }
    }
}