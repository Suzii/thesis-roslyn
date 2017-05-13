using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'Compare()' static methods called on strings and reports their usage when no overload with StringComparison or CultureInfo argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringCompareStaticMethodAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringCompareStaticMethodAnalyzer"/>
        /// </summary>
        public const string DiagnosticId = DiagnosticIds.StringCompareStaticMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringCompareStaticMethodAnalyzer"/> class.
        /// </summary>
        public StringCompareStaticMethodAnalyzer()
            : base("Compare")
        {
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule
            => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);
    }
}
