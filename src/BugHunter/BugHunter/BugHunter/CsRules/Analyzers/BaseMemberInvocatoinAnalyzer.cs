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
    /// Used for analysis of MemberAccess which Invocation at the same time such as "whereCondition.WhereLike(...)"
    /// </summary>
    public abstract class BaseMemberInvocatoinAnalyzer : DiagnosticAnalyzer
    {     
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, Type accessedType, string memberName, params string[] additionalMemberNames)
        {
            var forbiddenMemberNames = new[] {memberName}.Concat(additionalMemberNames);

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.InvocationExpression);
        }

        protected virtual  Location GetWarningLocation(InvocationExpressionSyntax invocationExpression)
        {
            return LocationHelper.GetLocationOfWholeInvocation(invocationExpression);
        }

        protected virtual string GetForbiddenUsageTextForUserMessage(InvocationExpressionSyntax invocationExpression)
        {
            var usedAs = $"{invocationExpression}";

            return usedAs;
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, Type accessedType, IEnumerable<string> forbiddenMemberNames)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
            
            // invocation expression is not part of member access
            if (memberAccess == null)
            {
                return;
            }
            
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

            var usedAs = GetForbiddenUsageTextForUserMessage(invocationExpression);
            var location = GetWarningLocation(invocationExpression);
            var diagnostic = Diagnostic.Create(rule, location, usedAs);

            context.ReportDiagnostic(diagnostic);
        }
    }
}