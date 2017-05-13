// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace BugHunter.Analyzers.StringAndCultureRules.CodeFixes
{
    /// <summary>
    /// Provides commonly used string comparison options
    /// </summary>
    internal static class StringComparisonOptions
    {
        /// <summary>
        /// Returns all commonly used string comparison options
        /// </summary>
        /// <returns>Commonly used string comparison options</returns>
        internal static IEnumerable<string> GetAll()
        {
            yield return "StringComparison.Ordinal";
            yield return "StringComparison.OrdinalIgnoreCase";
            yield return "StringComparison.CurrentCulture";
            yield return "StringComparison.CurrentCultureIgnoreCase";
            yield return "StringComparison.InvariantCulture";
            yield return "StringComparison.InvariantCultureIgnoreCase";
        }

        /// <summary>
        /// Returns case insensitive commonly used string comparison options
        /// </summary>
        /// <returns>Case insensitive commonly used string comparison options</returns>
        internal static IEnumerable<string> GetCaseInsensitive()
        {
            yield return "StringComparison.OrdinalIgnoreCase";
            yield return "StringComparison.CurrentCultureIgnoreCase";
            yield return "StringComparison.InvariantCultureIgnoreCase";
        }

        /// <summary>
        /// Returns case sensitive commonly used string comparison options
        /// </summary>
        /// <returns>Case sensitive commonly used string comparison options</returns>
        internal static IEnumerable<string> GetCaseSensitive()
        {
            yield return "StringComparison.Ordinal";
            yield return "StringComparison.CurrentCulture";
            yield return "StringComparison.InvariantCulture";
        }
    }
}