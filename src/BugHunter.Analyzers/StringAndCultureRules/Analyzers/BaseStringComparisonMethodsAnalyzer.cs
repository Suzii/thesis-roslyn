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

        private static readonly IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationOnlyFormatter();

        protected override IDiagnosticFormatter<InvocationExpressionSyntax> DiagnosticFormatter => _diagnosticFormatter;

        // If method is already called with StringComparison argument, no need for diagnostic
        protected override bool CheckPostConditions(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression)
        {
            var arguments = invocationExpression.ArgumentList.Arguments;

            return arguments.Any() && !arguments.Any(a => IsStringComparison(a, context) || IsCultureInfo(a, context));
        }
        
        private bool IsStringComparison(ArgumentSyntax argument, SyntaxNodeAnalysisContext context)
        {
            return IsOfType(argument, context, "StringComparison", "System.StringComparison");
        }

        private bool IsCultureInfo(ArgumentSyntax argument, SyntaxNodeAnalysisContext context)
        {
            return IsOfType(argument, context, "CultureInfo", "System.Globalization.CultureInfo");
        }

        private bool IsOfType(ArgumentSyntax argument, SyntaxNodeAnalysisContext context, string typeName, string typeFullName)
        {
            if (argument.Expression.ToString().Contains(typeName))
            {
                return true;
            }

            var argumentType = context.SemanticModel.GetTypeInfo(argument.Expression).Type as INamedTypeSymbol;
            if (argumentType == null)
            {
                return false;
            }

            var searchedTypeInfo = context.Compilation.GetTypeByMetadataName(typeFullName);
            return argumentType.Equals(searchedTypeInfo);
        }
    }
}
