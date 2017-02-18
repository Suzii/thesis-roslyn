using System.Linq;
using BugHunter.Core;
using BugHunter.Core.Analyzers;
using BugHunter.Core.DiagnosticsFormatting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    public abstract class BaseStringComparisonMethodsAnalyzer : BaseMemberInvocationAnalyzer
    {
        protected static DiagnosticDescriptor CreateRule(string diagnosticId)
        {
            return new DiagnosticDescriptor(diagnosticId,
                title: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Title), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                messageFormat: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_MessageFormat), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)),
                category: nameof(AnalyzerCategories.StringAndCulture),
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: new LocalizableResourceString(nameof(StringMethodsResources.StringComparisonMethods_Description), StringMethodsResources.ResourceManager, typeof(StringMethodsResources)));
        }

        protected override IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();
        }

        // If method is already called with StringComparison argument, no need for diagnostic
        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext expression, InvocationExpressionSyntax invocationExpression)
        {
            var arguments = invocationExpression.ArgumentList.Arguments;

            return arguments.Any() && !arguments.Any(a => IsStringComparison(a) || IsCultureInfo(a));
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
