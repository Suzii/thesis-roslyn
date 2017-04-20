using System;

namespace BugHunter.Core.Extensions
{
    /// <summary>
    /// Helper class containing extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtenstions
    {
        /// <summary>
        /// Checks whether <paramref name="source"/> contains <paramref name="toBeChecked"/> with respect to <paramref name="stringComparison"/>
        /// </summary>
        /// <param name="source">Source test to be searched for usage</param>
        /// <param name="toBeChecked">Usage to be searched for</param>
        /// <param name="stringComparison">String comparison to be used</param>
        /// <returns>True if <paramref name="source"/> contains <paramref name="toBeChecked"/></returns>
        public static bool Contains(this string source, string toBeChecked, StringComparison stringComparison)
        {
            return source.IndexOf(toBeChecked, stringComparison) >= 0;
        }
    }
}