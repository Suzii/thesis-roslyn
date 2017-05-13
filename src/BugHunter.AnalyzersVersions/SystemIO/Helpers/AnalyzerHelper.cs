// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;

namespace BugHunter.AnalyzersVersions.SystemIO.Helpers
{
    /// <summary>
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    public static class AnalyzerHelper
    {
        /// <summary>
        /// Names of whitelisted types for SystemIOAnalyzer
        /// </summary>
        public static readonly string[] WhiteListedTypeNames =
        {
            "System.IO.IOException",
            "System.IO.Stream",
            "System.IO.SeekOrigin"
        };

        /// <summary>
        /// Constructs a Diagnostic Descriptor for SystemIOAnalyzer
        /// </summary>
        /// <param name="diagnosticId">Id of the diagnostic</param>
        /// <returns>Diagnostic descriptor for SystemIOAnalyzer</returns>
        public static DiagnosticDescriptor GetRule(string diagnosticId) => new DiagnosticDescriptor(
            "BH1014",
            title: "Do not use System.IO",
            messageFormat: "'{0}' should not use 'System.IO' directly. Use equivalent method from namespace 'CMS.IO'.",
            category: nameof(AnalyzerCategories.CmsApiReplacements),
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "'System.IO' namespace should not be used directly. Use equivalent method from namespace 'CMS.IO'.");
    }
}