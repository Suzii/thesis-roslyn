using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.StringMethodsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'StartsWith()' and 'EndsWith()' methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringStartAndEndsWithMethodsAnalyzer : BaseStringComparisonMethodsAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS;

        private static readonly DiagnosticDescriptor StartsEndsWithRule = CreateRule(DIAGNOSTIC_ID);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(StartsEndsWithRule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(StartsEndsWithRule, context, "System.String", "StartsWith", "EndsWith");
        }
    }
}
