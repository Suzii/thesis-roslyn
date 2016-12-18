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
    /// Searches for usages of 'Equals()' and 'Compares()' etc. methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringComparisonMethodsAnalyzer : BaseMemberInvocationAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_COMPARISON_METHODS;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DIAGNOSTIC_ID,
            title: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Title), CsResources.ResourceManager, typeof(CsResources)),
            messageFormat: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_MessageFormat), CsResources.ResourceManager, typeof(CsResources)),
            category: AnalyzerCategories.CS_RULES,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(CsResources.StringComparisonMethods_Description), CsResources.ResourceManager, typeof(CsResources)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            RegisterAction(Rule, context, "System.String",
                "Equals",
                "StartsWith",
                "EndsWith",
                "IndexOf",
                "LastIndexOf");

            // compareTo must be diagnosed as different diagnostic since
            // it does not have overload instance overload, only static, 
            // so codefix needs to distinguish this one
            RegisterAction(Rule, context, "System.String", "CompareTo");
        }

        // If method is already called with StringComparison argument, no need for diagnostic
        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext expression, InvocationExpressionSyntax invocationExpression)
        {
            return !invocationExpression.ArgumentList.Arguments
                .Any(a => IsStringComparison(a) || IsCultureInfo(a));
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
