using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'StartsWith()' and 'EndsWith()' methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringStartAndEndsWithMethodsAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringStartAndEndsWithMethodsAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.StringStartsEndsWithMethods;

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule
            => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringStartAndEndsWithMethodsAnalyzer"/> class.
        /// </summary>
        public StringStartAndEndsWithMethodsAnalyzer()
            : base("StartsWith", "EndsWith")
        {
        }
    }
}
