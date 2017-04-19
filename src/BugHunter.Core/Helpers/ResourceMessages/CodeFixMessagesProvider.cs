using Microsoft.CodeAnalysis;

namespace BugHunter.Core.Helpers.ResourceMessages
{
    /// <summary>
    /// Helper class for constructing code fix messages 
    /// </summary>
    public static class CodeFixMessagesProvider
    {
        /// <summary>
        /// Get message for code fix of API replacement analyzer
        /// </summary>
        /// <param name="codeFix">Message argument containing code fix to be introduced</param>
        /// <returns>Code fix message</returns>
        public static string GetReplaceWithMessage(string codeFix)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_CodeFix), Resources.ResourceManager, typeof(Resources), codeFix).ToString();
        }

        /// <summary>
        /// Get message for code fix of API replacement analyzer
        /// </summary>
        /// <param name="codeFix">Message argument containing syntax node to be introduced as code fix</param>
        /// <returns>Code fix message</returns>
        public static string GetReplaceWithMessage(SyntaxNode codeFix)
        {
            return GetReplaceWithMessage(codeFix.ToString());
        }

        /// <summary>
        /// Get message for code fix for CMS Base Classes analyzers that inherit from wrong class
        /// </summary>
        /// <param name="baseClass">Message argument containing code fix to be introduced</param>
        /// <returns>Code fix message</returns>
        public static string GetInheritFromMessage(string baseClass)
        {
            return new LocalizableResourceString(nameof(Resources.BaseClasses_InheritFrom_CodeFix), Resources.ResourceManager, typeof(Resources), baseClass).ToString();
        }
    }
}