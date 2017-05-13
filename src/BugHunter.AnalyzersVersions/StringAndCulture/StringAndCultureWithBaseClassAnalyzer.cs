using System.Collections.Immutable;
using System.Linq;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.StringAndCulture
{
    /// <summary>
    /// Test version of StringAndCulture analyzer that uses the base class optimized approach
    ///
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    //[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class StringAndCultureWithBaseClassAnalyzer : BaseStringMethodsAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BH4004";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        protected override DiagnosticDescriptor Rule => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);

        public StringAndCultureWithBaseClassAnalyzer()
            : base("IndexOf", "LastIndexOf")
        {
        }

        protected override bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            => base.IsForbiddenOverload(context, invocationExpression, methodSymbol) && !IsFirstArgumentChar(methodSymbol);

        private static bool IsFirstArgumentChar(IMethodSymbol methodSymbol)
            => !methodSymbol.Parameters.IsEmpty && methodSymbol.Parameters.First().Type.SpecialType == SpecialType.System_Char;
    }
}
