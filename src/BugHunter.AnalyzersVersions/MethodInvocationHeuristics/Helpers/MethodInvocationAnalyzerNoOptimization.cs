using System.Linq;
using BugHunter.Core.Analyzers;
using BugHunter.Core.ApiReplacementAnalysis;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.AnalyzersVersions.MethodInvocationHeuristics.Helpers
{
    /// <summary>
    /// This is duplicate version of <see cref="MethodInvocationAnalyzer"/> BUT withou syntax heuristics optimization.
    ///
    /// !!! THIS FILE SERVES ONLY FOR PURPOSES OF PERFORMANCE TESTING !!!
    /// </summary>
    public class MethodInvocationAnalyzerNoOptimization : ISyntaxNodeAnalyzer
    {
        private readonly ApiReplacementConfig _config;
        private readonly ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInvocationAnalyzerNoOptimization"/> class.
        /// </summary>
        /// <param name="config">Configuration for the analysis</param>
        /// <param name="formatter">Diagnostic formatter</param>
        public MethodInvocationAnalyzerNoOptimization(ApiReplacementConfig config, ISyntaxNodeDiagnosticFormatter<InvocationExpressionSyntax> formatter)
        {
            _config = config;
            _formatter = formatter;
        }

        /// <inheritdoc/>
        public void Run(SyntaxNodeAnalysisContext context)
        {
            var filePath = context.Node?.SyntaxTree?.FilePath;
            if (!IsOnForbiddenPath(filePath))
            {
                return;
            }

            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation == null)
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