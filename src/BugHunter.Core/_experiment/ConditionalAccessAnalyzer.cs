using System;
using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core._experiment
{
    public class ConditionalAccessAnalyzer : IAccessAnalyzer
    {
        protected readonly ApiReplacementConfig Config;
        protected readonly IDiagnosticFormatter<ConditionalAccessExpressionSyntax> Formatter;

        public ConditionalAccessAnalyzer(ApiReplacementConfig config) 
            : this(config, DiagnosticFormatterFactory.CreateConditionalAccessFormatter())
        {
        }

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

            var diagnostic = CreateDiagnostic(conditionalAccess);
            context.ReportDiagnostic(diagnostic);
        }

        protected bool IsForbiddenUsage(SyntaxNodeAnalysisContext context, ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var whereNotNull = conditionalAccess.WhenNotNull.ToString();
            if (Config.ForbiddenMembers.All(forbiddenMember => !whereNotNull.StartsWith("."+forbiddenMember, StringComparison.Ordinal)))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(conditionalAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType != null && Config.ForbiddenTypes.Any(forbidenType => actualTargetType.IsDerivedFrom(forbidenType, context.Compilation));
        }

        protected Diagnostic CreateDiagnostic(ConditionalAccessExpressionSyntax conditionalAccess)
        {
            var location = Formatter.GetLocation(conditionalAccess);
            var diagnosedUsage = Formatter.GetDiagnosedUsage(conditionalAccess);
            var diagnostic = Diagnostic.Create(Config.Rule, location, diagnosedUsage);

            return diagnostic;
        }
    }
}