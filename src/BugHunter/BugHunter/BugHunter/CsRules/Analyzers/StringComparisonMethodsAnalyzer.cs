using System.Collections.Immutable;
using System.Linq;
using BugHunter.Core;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'Equals()' and 'CompareTo()' etc. methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringComparisonMethodsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID_EQUALS_COMPARE = DiagnosticIds.STRING_EQUALS_COMPARE_TO_METHODS;
        public const string DIAGNOSTIC_ID_EQUALS_COMPARE_STATIC = DiagnosticIds.STRING_EQUALS_COMPARE_STATIC_METHODS;
        public const string DIAGNOSTIC_ID_START_ENDS_WITH = DiagnosticIds.STRING_STARTS_ENDS_WITH_METHODS;
        public const string DIAGNOSTIC_ID_INDEX_OF = DiagnosticIds.STRING_INDEX_OF_METHODS;

        private static readonly DiagnosticDescriptor EqualsCompareRule = new DiagnosticDescriptor(DIAGNOSTIC_ID_EQUALS_COMPARE,
            title: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Description), CsResources.ResourceManager, typeof(CsResources)));

        private static readonly DiagnosticDescriptor EqualsCompareStaticRule = new DiagnosticDescriptor(DIAGNOSTIC_ID_EQUALS_COMPARE_STATIC,
            title: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Description), CsResources.ResourceManager, typeof(CsResources)));

        private static readonly DiagnosticDescriptor StartsEndsWithRule = new DiagnosticDescriptor(DIAGNOSTIC_ID_START_ENDS_WITH,
            title: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Description), CsResources.ResourceManager, typeof(CsResources)));

        private static readonly DiagnosticDescriptor IndexOfRule = new DiagnosticDescriptor(DIAGNOSTIC_ID_INDEX_OF,
            title: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(EqualsCompareRule, EqualsCompareStaticRule, IndexOfRule, StartsEndsWithRule);

        public override void Initialize(AnalysisContext context)
        {
            // compareTo must be diagnosed as different diagnostic since
            // it does not have overload instance overload, only static, 
            // so codefix needs to distinguish this one
            RegisterAction(EqualsCompareRule, context, "System.String", "Equals", "CompareTo");
            RegisterAction(StartsEndsWithRule, context, "System.String", "StartsWith", "EndsWith");
            RegisterAction(IndexOfRule, context, "System.String", "IndexOf", "LastIndexOf");

            // TODO static variant for Equals and Compare
        }

        // If method is already called with StringComparison argument, no need for diagnostic
        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext expression, InvocationExpressionSyntax invocationExpression)
        {
            var arguments = invocationExpression.ArgumentList.Arguments;

            return arguments.Any() && 
                !arguments.First().Expression.ToString().Trim().StartsWith("'") && 
                !arguments.Any(a => IsStringComparison(a) || IsCultureInfo(a));
        }
        
        protected override IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();
        }

        // TODO check for proper type not just string
        private bool IsStringComparison(ArgumentSyntax argument)
        {
            return argument.Expression.ToString().Contains("StringComparison");
        }

        // TODO check for proper type not just string
        private bool IsCultureInfo(ArgumentSyntax argument)
        {
            return argument.Expression.ToString().Contains("CultureInfo");
        }
    }
}
