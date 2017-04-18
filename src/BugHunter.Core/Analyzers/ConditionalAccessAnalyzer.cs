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
    public class ConditionalAccessAnalyzer : ISyntaxNodeAnalyzer
    {
        private readonly ApiReplacementConfig _config;
        private readonly ISyntaxNodeDiagnosticFormatter<ConditionalAccessExpressionSyntax> _formatter;
        
        public ConditionalAccessAnalyzer(ApiReplacementConfig config, ISyntaxNodeDiagnosticFormatter<ConditionalAccessExpressionSyntax> formatter)
        {
            _config = config;
            _formatter = formatter;
        }

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
            var whereNotNull = conditionalAccess.WhenNotNull.ToString();
            // TODO what if usage is just prefix of forbidden member name
            if (_config.ForbiddenMembers.All(forbiddenMember => !whereNotNull.StartsWith("."+forbiddenMember, StringComparison.Ordinal)))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(conditionalAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType != null && _config.ForbiddenTypes.Any(forbidenType => actualTargetType.IsDerivedFrom(forbidenType, context.Compilation));
        }
    }
}