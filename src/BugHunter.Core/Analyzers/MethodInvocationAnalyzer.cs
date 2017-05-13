using System.Linq;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Analyzers
{
    /// <summary>
    /// Analyzing strategy for <see cref="InvocationExpressionSyntax"/>
    ///
    /// Runs the analysis for current context based on the <see cref="ApiReplacementConfig"/>
    /// and raises diagnostics using passed <see cref="ISyntaxNodeDiagnosticFormatter{TSyntaxNode}"/>
    /// </summary>
    public class MethodInvocationAnalyzer : ISyntaxNodeAnalyzer
    {
        private readonly ApiReplacementConfig _config;
        private readonly ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationAnalyzer"/> class.
        /// </summary>
        /// <param name="config">Configuration to be used for the analysis, specifying forbidden members on specific types and diagnostic descriptor</param>
        /// <param name="formatter">Diagnostic formatter to be used for creation of diagnostic</param>
        public MethodInvocationAnalyzer(ApiReplacementConfig config, ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> formatter)
        {
            _config = config;
            _formatter = formatter;
        }

        /// <summary>
        /// Run the analysis on current syntax node context and raise the diagnostic if forbidden invocations are found
        /// </summary>
        /// <param name="context">Current syntax node analysis context</param>
        public void Run(SyntaxNodeAnalysisContext context)
        {
            var filePath = context.Node?.SyntaxTree?.FilePath;
            if (!IsOnForbiddenPath(filePath))
            {
                return;
            }

            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation == null || CanBeSkippedBasedOnSyntaxOnly(invocation))
            {
                return;
            }

            var invokedMethodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (invokedMethodSymbol == null || !IsForbiddenMethodOnForbiddenType(invokedMethodSymbol, context.Compilation))
            {
                return;
            }

            if (!IsForbiddenUsage(invocation, invokedMethodSymbol))
            {
                return;
            }

            var diagnostic = _formatter.CreateDiagnostic(_config.Rule, invocation);
            context.ReportDiagnostic(diagnostic);
        }

        /// <summary>
        /// Determines whether the analysis should continue based on the document path
        /// </summary>
        /// <param name="filePath">File path of the document</param>
        /// <returns>True if the path is considered forbiden in context of the analysis; false otherwise</returns>
        protected virtual bool IsOnForbiddenPath(string filePath) => true;

        /// <summary>
        /// Determines, whether the analysis should continue based on the inspected
        /// <paramref name="invocation"/> and its corresponding <paramref name="methodSymbol"/>
        /// </summary>
        /// <param name="invocation">Inspected invocation</param>
        /// <param name="methodSymbol">Method symbol corresponding to the invocation</param>
        /// <returns>True if the inspected invocation is considered forbidden; false otherwise</returns>
        protected virtual bool IsForbiddenUsage(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol) => true;

        private bool CanBeSkippedBasedOnSyntaxOnly(InvocationExpressionSyntax invocationExpression)
        {
            SimpleNameSyntax methodNameNode;
            if (!invocationExpression.TryGetMethodNameNode(out methodNameNode))
            {
                return false;
            }

            return !IsForbiddenMethod(methodNameNode.Identifier.ValueText);
        }

        private bool IsForbiddenMethodOnForbiddenType(IMethodSymbol methodSymbol, Compilation compilation)
        {
            if (!IsForbiddenMethod(methodSymbol.Name))
            {
                return false;
            }

            var receiverTypeSymbol = methodSymbol.ReceiverType as INamedTypeSymbol;
            return receiverTypeSymbol != null && IsForbiddenType(receiverTypeSymbol, compilation);
        }

        private bool IsForbiddenMethod(string methodName)
            => _config.ForbiddenMembers.Contains(methodName);

        private bool IsForbiddenType(INamedTypeSymbol receiverTypeSymbol, Compilation compilation)
            => _config.ForbiddenTypes.Any(forbiddenType => receiverTypeSymbol.IsDerivedFrom(forbiddenType, compilation));
    }
}