using System;
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
    /// Analyzing strategy for <see cref="ConditionalAccessExpressionSyntax"/>
    ///
    /// Runs the analysis for current context based on the <see cref="ApiReplacementConfig"/>
    /// and raises diagnostics using passed <see cref="ISyntaxNodeDiagnosticFormatter{TSyntaxNode}"/>
    /// </summary>
    public class ConditionalAccessAnalyzer : ISyntaxNodeAnalyzer
    {
        private readonly ApiReplacementConfig _config;
        private readonly ISyntaxNodeDiagnosticFormatter<ConditionalAccessExpressionSyntax> _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalAccessAnalyzer"/> class.
        /// </summary>
        /// <param name="config">Configuration to be used for the analysis, specifying forbidden members on specific types and diagnostic descriptor</param>
        /// <param name="formatter">Diagnostic formatter to be used for creation of diagnostic</param>
        public ConditionalAccessAnalyzer(ApiReplacementConfig config, ISyntaxNodeDiagnosticFormatter<ConditionalAccessExpressionSyntax> formatter)
        {
            _config = config;
            _formatter = formatter;
        }

        /// <summary>
        /// Runs the analysis for current <paramref name="context"/> and raises <see cref="Diagnostic"/> if usage is qualified as forbidden
        /// </summary>
        /// <param name="context">Context to perform analysis on</param>
        public void Run(SyntaxNodeAnalysisContext context)
        {
            var conditionalAccess = (ConditionalAccessExpressionSyntax)context.Node;
            if (conditionalAccess == null)
            {
                return;
            }

            if (!IsForbiddenUsage(context, conditionalAccess))
            {
                return;
            }

            var diagnostic = _formatter.CreateDiagnostic(_config.Rule, conditionalAccess);
            context.ReportDiagnostic(diagnostic);
        }

        private bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var memberName = conditionalAccess.GetFirstMemberBindingExpression()?.Name?.Identifier.ValueText;
            if (!string.IsNullOrEmpty(memberName) &&
                _config.ForbiddenMembers.All(forbiddenMember => !memberName.Equals(forbiddenMember, StringComparison.Ordinal)))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(conditionalAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType != null && _config.ForbiddenTypes.Any(forbidenType => actualTargetType.IsDerivedFrom(forbidenType, context.Compilation));
        }
    }
}