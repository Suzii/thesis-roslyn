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
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS;

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule 
            => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DIAGNOSTIC_ID);

        /// <summary>
        /// Constructor initializing base class with method names to be diagnosed
        /// </summary>
        public StringStartAndEndsWithMethodsAnalyzer() 
            : base("StartsWith", "EndsWith")
        {
        }
    }
}
