using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace BugHunter.Core._experiment
{
    public class ApiReplacementConfig
    {
        public ApiReplacementConfig(DiagnosticDescriptor rule, IImmutableSet<string> forbiddenTypes, IImmutableSet<string> forbiddenMembers)
        {
            Rule = rule;
            ForbiddenTypes = forbiddenTypes;
            ForbiddenMembers = forbiddenMembers;
        }

        public DiagnosticDescriptor Rule { get; }

        public IImmutableSet<string> ForbiddenTypes { get; }

        public IImmutableSet<string> ForbiddenMembers { get; }
    }
}