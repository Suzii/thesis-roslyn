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
    // [DiagnosticAnalyzer(LanguageNames.CSharp)]
#pragma warning disable RS1001 // Missing diagnostic analyzer attribute.
    public class StringAndCultureWithStrategyClassAnalyzer : DiagnosticAnalyzer
#pragma warning restore RS1001 // Missing diagnostic analyzer attribute.
    {
        public const string DiagnosticId = "BH4004";

        private static readonly ApiReplacementConfig Config = new ApiReplacementConfig(
            Rule,
            new[] { "System.String" },
            new[] { "IndexOf", "LastIndexOf" });

        private static readonly ISyntaxNodeAnalyzer Analyzer = new InnerMethodInvocationAnalyzer(Config);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(Rule);

        private static DiagnosticDescriptor Rule
            => StringMethodsRuleBuilder.CreateRuleForComparisonMethods(DiagnosticId);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyzer.Run, SyntaxKind.InvocationExpression);
        }

        internal class InnerMethodInvocationAnalyzer : MethodInvocationAnalyzer
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InnerMethodInvocationAnalyzer"/> class.
            /// </summary>
            /// <param name="config">Configuration for analysis</param>
            public InnerMethodInvocationAnalyzer(ApiReplacementConfig config)
                : base(config, new MethodInvocationOnlyDiagnosticFormatter())
            {
            }

            /// <inheritdoc />
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
