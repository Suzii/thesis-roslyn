using Microsoft.CodeAnalysis;

namespace BugHunter.Core.ResourceBuilder
{
    public static class CodeFixMessageBuilder
    {
        public static string GetReplaceWithMessage(string codeFix)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_CodeFix), Resources.ResourceManager, typeof(Resources), codeFix).ToString();
        }

        public static string GetReplaceWithMessage(SyntaxNode codeFix)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_CodeFix), Resources.ResourceManager, typeof(Resources), codeFix.ToString()).ToString();
        }

        public static string GetInheritFromMessage(string baseClass)
        {
            return new LocalizableResourceString(nameof(Resources.BaseClasses_InheritFrom_CodeFix), Resources.ResourceManager, typeof(Resources), baseClass).ToString();
        }
    }
}