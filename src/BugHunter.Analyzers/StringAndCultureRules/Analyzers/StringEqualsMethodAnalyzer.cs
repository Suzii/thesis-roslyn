using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'Equals()' static methods called on strings and reports their usage when no overload with StringComparison or CultureInfo argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringEqualsMethodAnalyzer : BaseStringMethodsAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_EQUALS_METHOD;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        protected override DiagnosticDescriptor Rule => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DIAGNOSTIC_ID);

        public StringEqualsMethodAnalyzer() 
            : base("Equals")
        {
        }
    }
}
