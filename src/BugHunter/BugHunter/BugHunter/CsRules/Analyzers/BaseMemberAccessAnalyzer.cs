using System;
using System.Collections.Generic;
using System.Linq;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using BugHunter.Core.ResourceBuilder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Used for analysis of SimpleMemberAccess such as "page.IsCallback"
    /// </summary>
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
        
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, Type accessedType, string memberName, params string[] additionalMemberNames)
        {
            var forbiddenMemberNames = new[] {memberName}.Concat(additionalMemberNames);

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.SimpleMemberAccessExpression);
        }

        protected virtual  Location GetWarningLocation(MemberAccessExpressionSyntax memberAccess)
        {
            var location = memberAccess.GetLocation();

            return location;
        }

        protected virtual string GetForbiddenUsageTextForUserMessage(MemberAccessExpressionSyntax memberAccess)
        {
            var usedAs = $"{memberAccess.Expression}.{memberAccess.Name}";

            return usedAs;
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, Type accessedType, IEnumerable<string> forbiddenMemberNames)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;

            var memberName = memberAccess.Name.ToString();
            if (!forbiddenMemberNames.Contains(memberName))
            {
                return;
            }

            var searchedTargetType = accessedType.GetITypeSymbol(context);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (searchedTargetType == null || actualTargetType == null || !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return;
            }

            var usedAs = GetForbiddenUsageTextForUserMessage(memberAccess);
            var location = GetWarningLocation(memberAccess);
            var diagnostic = Diagnostic.Create(rule, location, usedAs);

            context.ReportDiagnostic(diagnostic);
        }
    }
}