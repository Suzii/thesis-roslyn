using System.Collections.Immutable;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers
{
    /// <summary>
    /// Searches for usages of 'ToLower()' and 'ToUpper()' methods called on strings and reports their usage when no overload with StringComparison argument is used
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StringManipulationMethodsAnalyzer : BaseStringMethodsAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics raises by <see cref="StringManipulationMethodsAnalyzer"/>
        /// </summary>
        public const string DIAGNOSTIC_ID = DiagnosticIds.STRING_MANIPULATION_METHODS;

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        /// <inheritdoc />
        protected override DiagnosticDescriptor Rule 
            => StringMethodsRuleBuilder.CreateRuleForManipulationMethods(DIAGNOSTIC_ID);

        /// <inheritdoc />
        protected override bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            // If method is already called with StringComparison argument, no need for diagnostic
            return invocationExpression.ArgumentList.Arguments.Count == 0;
        }

        /// <summary>
        /// Constructor initializing base class with method names to be diagnosed
        /// </summary>
        public StringManipulationMethodsAnalyzer()
            : base("ToLower", "ToUpper")
        {
        }
    }
}
