using System;
using BugHunter.Core.Helpers.Analyzers;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    public abstract class BaseMemberAccessAnalyzer : DiagnosticAnalyzer
    {
        protected static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage)
        {
            var rule = new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessageBuilder.GetTitle(forbiddenUsage),
                messageFormat: ApiReplacementsMessageBuilder.GetMessageFormat(),
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessageBuilder.GetDescription(forbiddenUsage));

            return rule;
        }

        protected static DiagnosticDescriptor GetRule(string diagnosticId, string forbiddenUsage, string recommendedUsage)
        {
            var rule = new DiagnosticDescriptor(diagnosticId,
                title: ApiReplacementsMessageBuilder.GetTitle(forbiddenUsage, recommendedUsage),
                messageFormat: ApiReplacementsMessageBuilder.GetMessageFormat(recommendedUsage),
                category: AnalyzerCategories.CS_RULES,
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: ApiReplacementsMessageBuilder.GetDescription(forbiddenUsage, recommendedUsage));

            return rule;
        }
        
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, Type accessedType, params string[] memberNames)
        {
            if (memberNames.Length == 0)
            {
                return;
            }

            var analyzer = new MemberAccessAnalysisHelper(rule, accessedType, memberNames);

            context.RegisterSyntaxNodeAction(c => analyzer.Analyze(c), SyntaxKind.SimpleMemberAccessExpression);
        }
    }
}