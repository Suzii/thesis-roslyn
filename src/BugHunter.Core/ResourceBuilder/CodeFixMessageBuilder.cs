using Microsoft.CodeAnalysis;

namespace BugHunter.Core.ResourceBuilder
{
    public static class CodeFixMessageBuilder
    {
        public static string GetMessage(string codeFix)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_CodeFix), Resources.ResourceManager, typeof(Resources), codeFix).ToString();
        }

        public static string GetMessage(SyntaxNode codeFix)
        {
            return new LocalizableResourceString(nameof(Resources.ApiReplacements_CodeFix), Resources.ResourceManager, typeof(Resources), codeFix.ToString()).ToString();
        }
    }
}