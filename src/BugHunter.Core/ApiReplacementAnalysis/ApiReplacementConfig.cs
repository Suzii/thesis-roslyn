// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.CodeAnalysis;

namespace BugHunter.Core.ApiReplacementAnalysis
{
    /// <summary>
    /// Configuration class for ApiReplacement analyzers
    /// It defines <see cref="ForbiddenTypes"/> and their <see cref="ForbiddenMembers"/> that should be analyzed,
    /// and upon which diagnostic specified by <see cref="Rule"/> should be raised
    /// </summary>
    public class ApiReplacementConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiReplacementConfig"/> class.
        /// </summary>
        /// <param name="rule">Diagnostic descriptor</param>
        /// <param name="forbiddenTypes">Fully qualified names of forbidden types</param>
        /// <param name="forbiddenMembers">Names of forbidden members of type</param>
        public ApiReplacementConfig(DiagnosticDescriptor rule, string[] forbiddenTypes, string[] forbiddenMembers)
        {
            Rule = rule;
            ForbiddenTypes = forbiddenTypes;
            ForbiddenMembers = forbiddenMembers;
        }

        /// <summary>
        /// Gets the <see cref="DiagnosticDescriptor"/> to be used when diagnostic should be raised
        /// </summary>
        public DiagnosticDescriptor Rule { get; }

        /// <summary>
        /// Gets fully qualified names of types whose <see cref="ForbiddenMembers"/> should be diagnosed
        /// </summary>
        public string[] ForbiddenTypes { get; }

        /// <summary>
        /// Gets member names of <see cref="ForbiddenTypes"/> that should be diagnosed
        /// </summary>
        public string[] ForbiddenMembers { get; }
    }
}