using System.Collections.Immutable;
using System.Linq;
using BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.StringAndCulture
{
    /// <summary>
    /// Test version of StringAndCulture analyzer that uses the MethodInvocationAnalyzer strategy pattern
    /// 
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    //[DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class StringAndCultureWithStrategyClassAnalyzer : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BH4004";
        
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static DiagnosticDescriptor Rule => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);

        private static readonly ApiReplacementConfig config = new ApiReplacementConfig(Rule,
            new[] { "System.String" },
            new[] { "IndexOf", "LastIndexOf" });

        private static readonly ISyntaxNodeAnalyzer analyzer = new InnerMethodInvocationAnalyzer(config);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(analyzer.Run, SyntaxKind.InvocationExpression);
        }

        internal class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            public InnerMethodInvocationAnalyzer(ApiReplacementConfig config) : base(config, new MethodInvocationOnlyDiagnosticFormatter())
            {
            }

            protected override bool IsForbiddenUsage(InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            => methodSymbol.Parameters.All(argument => !IsStringComparison(argument) && !IsCultureInfo(argument)) && !IsFirstArgumentChar(methodSymbol);

            private static bool IsStringComparison(IParameterSymbol parameter)
                => parameter.Type.ToDisplayString() == "System.StringComparison";

            private static bool IsCultureInfo(IParameterSymbol parameter)
                => parameter.Type.ToDisplayString() == "System.Globalization.CultureInfo";

            private static bool IsFirstArgumentChar(IMethodSymbol methodSymbol)
                => !methodSymbol.Parameters.IsEmpty && methodSymbol.Parameters.First().Type.SpecialType == SpecialType.System_Char;
        }
    }
}
