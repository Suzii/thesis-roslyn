using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'IndexOf()' and 'LastIndexOf()' etc. methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringIndexOfMethodsAnalyzer : BaseStringMethodsAnalyzer
    {
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_INDEX_OF_METHODS;
        
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        protected override DiagnosticDescriptor Rule => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DIAGNOSTIC_ID);

        public StringIndexOfMethodsAnalyzer()
            : base("IndexOf", "LastIndexOf")
        {
        }

        protected override bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            return base.IsForbiddenOverload(context, invocationExpression, methodSymbol) && !IsFirstArgumentChar(context, invocationExpression, methodSymbol);
        }

        private static bool IsFirstArgumentChar(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            var firstArgument = invocationExpression
                .ArgumentList
                .Arguments
                .FirstOrDefault()
                ?.Expression;

            if (firstArgument == null)
            {
                return false;
            }

            if (firstArgument.ToString().StartsWith("'"))
            {
                return true;
            }

            // it can be a variable of type char
            var firstArgumentType = context.SemanticModel.GetTypeInfo(firstArgument).Type;

            return firstArgumentType?.SpecialType == SpecialType.System_Char;
        }

        
    }
}
