using System.Collections.Immutable;
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
        public ApiReplacementConfig(DiagnosticDescriptor rule, string[] forbiddenTypes, string[] forbiddenMembers)
        {
            Rule = rule;
            ForbiddenTypes = forbiddenTypes;
            ForbiddenMembers = forbiddenMembers;
        }

        /// <summary>
        /// The <see cref="DiagnosticDescriptor"/> to be used when diagnostic should be raised
        /// </summary>
        public DiagnosticDescriptor Rule { get; }

        /// <summary>
        /// Fully qualified names of types whose <see cref="ForbiddenMembers"/> should be diagnosed
        /// </summary>
        public string[] ForbiddenTypes { get; }

        /// <summary>
        /// Member names of <see cref="ForbiddenTypes"/> that should be diagnosed
        /// </summary>
        public string[] ForbiddenMembers { get; }
    }
}