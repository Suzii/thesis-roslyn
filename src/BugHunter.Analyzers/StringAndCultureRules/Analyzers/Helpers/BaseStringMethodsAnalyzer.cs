using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.DiagnosticsFormatting.Implementation;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Analyzers.StringAndCultureRules.Analyzers.Helpers
{
    /// <summary>
    /// Base class for all analyzers of string methods
    /// </summary>
    public abstract class BaseStringMethodsAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Diagnostic Descriptor that should be used for diagnostic creation
        /// </summary>
        protected abstract DiagnosticDescriptor Rule { get; }

        private readonly string[] _forbiddenMethods;

        private static readonly ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> DiagnosticFormatter = new MethodInvocationOnlyDiagnosticFormatter();

        /// <summary>
        /// Constructor accepting one or more names of string methods that are considered forbidden and diagnostic should be raised for them
        /// </summary>
        /// <param name="forbiddenMethod">Name of the forbidden method</param>
        /// <param name="additionalForbiddenMethods">Names of additional forbidden methods</param>
        protected BaseStringMethodsAnalyzer(string forbiddenMethod, params string[] additionalForbiddenMethods)
        {
            _forbiddenMethods = additionalForbiddenMethods.Concat(new []{ forbiddenMethod }).ToArray();
        }

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        /// <summary>
        /// Determines whether the passed <param name="invocation"></param> corresponding to <param name="invokedMethodSymbol"></param> is considered forbidden
        /// </summary>
        /// <param name="context">Current syntax node analysis context</param>
        /// <param name="invocation">Inspected invocation</param>
        /// <param name="invokedMethodSymbol">Method symbol corresponding to the inspected invocation</param>
        /// <returns>True if passed invocation is considered forbidden; false otherwise</returns>
        protected bool IsForbiddenStringMethod(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol invokedMethodSymbol)
            => IsInvokedOnString(invokedMethodSymbol) && IsForbiddenMethodName(invokedMethodSymbol?.Name);

        /// <summary>
        /// Determines whether the invoked string method is considered as forbidden based on current context
        /// </summary>
        /// <remarks>
        /// If the method is not invoked with <see cref="System.StringComparison"/> or <see cref="System.Globalization.CultureInfo"/> argument, it is considered as forbidden
        /// </remarks>
        /// <param name="context">Current syntax node analysis context</param>
        /// <param name="invocation">Inspected invocation</param>
        /// <param name="invokedMethodSymbol">Method symbol corresponding to the inspected invocation</param>
        /// <returns>True if passed invocation is considered forbidden; false otherwise</returns>
        protected virtual bool IsForbiddenOverload(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol invokedMethodSymbol)
            => invokedMethodSymbol.Parameters.All(argument => !IsStringComparison(argument) && !IsCultureInfo(argument));

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            if (invocationExpression == null || CanBeSkippedBasedOnSyntaxOnly(invocationExpression))
            {
                return;
            }
            
            var invokedMethodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (invokedMethodSymbol == null || !IsForbiddenStringMethod(context, invocationExpression, invokedMethodSymbol))
            {
                return;
            }

            if (!IsForbiddenOverload(context, invocationExpression, invokedMethodSymbol))
            {
                return;
            }

            var diagnostic = DiagnosticFormatter.CreateDiagnostic(Rule, invocationExpression);
            context.ReportDiagnostic(diagnostic);
        }

        private bool CanBeSkippedBasedOnSyntaxOnly(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return false;
            }

            return !IsForbiddenMethodName(methodNameNode.Identifier.ValueText);
        }

        private bool IsForbiddenMethodName(string invokedMethodName)
           => _forbiddenMethods.Contains(invokedMethodName);

        private bool IsInvokedOnString(IMethodSymbol invokedMethodSymbol)
            => invokedMethodSymbol?.ReceiverType?.SpecialType == SpecialType.System_String;

        private bool IsStringComparison(IParameterSymbol parameter)
            => parameter.Type.ToDisplayString() == "System.StringComparison";

        private bool IsCultureInfo(IParameterSymbol parameter)
            => parameter.Type.ToDisplayString() == "System.Globalization.CultureInfo";
    }
}