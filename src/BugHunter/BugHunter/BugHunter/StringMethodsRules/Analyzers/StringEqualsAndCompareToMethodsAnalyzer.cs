using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.StringMethodsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'Equals()' and 'CompareTo()' etc. methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringEqualsAndCompareToMethodsAnalyzer : BaseStringComparisonMethodsAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_EQUALS_COMPARE_TO_METHODS;


        private static readonly DiagnosticDescriptor Rule = CreateRule(DIAGNOSTIC_ID);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.String", "Equals", "CompareTo");
        }
    }
}
