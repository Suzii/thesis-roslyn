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
        protected readonly ApiReplacementConfig Config;
        protected readonly IDiagnosticFormatter<ConditionalAccessExpressionSyntax> Formatter;
        
        public ConditionalAccessAnalyzer(ApiReplacementConfig config, IDiagnosticFormatter<ConditionalAccessExpressionSyntax> formatter)
        {
            Config = config;
            Formatter = formatter;
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

            var diagnostic = Formatter.CreateDiagnostic(Config.Rule, conditionalAccess);
            context.ReportDiagnostic(diagnostic);
        }

        protected bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var whereNotNull = conditionalAccess.WhenNotNull.ToString();
            // TODO what if usage is just prefix of forbidden member name
            if (Config.ForbiddenMembers.All(forbiddenMember => !whereNotNull.StartsWith("."+forbiddenMember, StringComparison.Ordinal)))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(conditionalAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType != null && Config.ForbiddenTypes.Any(forbidenType => actualTargetType.IsDerivedFrom(forbidenType, context.Compilation));
        }
    }
}