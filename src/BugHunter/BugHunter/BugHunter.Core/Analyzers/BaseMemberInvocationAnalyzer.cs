using System.Collections.Generic;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.Core.Analyzers
{
    /// <summary>
    /// Used for analysis of MemberAccess which Invocation at the same time such as "whereCondition.WhereLike(...)"
    /// </summary>
    public abstract class BaseMemberInvocationAnalyzer : DiagnosticAnalyzer
    {
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string methodName, params string[] additionalMethodNames)
        {
            var forbiddenMemberNames = new HashSet<string>(additionalMethodNames) { methodName };

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.InvocationExpression);
        }

        protected virtual IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberInvocationFormatter();
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, ISet<string> forbiddenMethodNames)
        {
            if (!CheckPreConditions(context))
            {
                return;
            }

            if (!CheckMainConditions(context, accessedType, forbiddenMethodNames))
            {
                return;
            }

            var invocationExpression = context.Node as InvocationExpressionSyntax;
            if (!CheckPostConditions(context, invocationExpression))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(rule, invocationExpression);

            context.ReportDiagnostic(diagnostic);
        }

        // Can be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPreConditions(SyntaxNodeAnalysisContext context)
        {
            // TODO check if file is generated
            return true;
        }

        protected virtual bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, ISet<string> methodNames)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;

            // invocation expression is not part of member access
            if (memberAccess == null)
            {
                return false;
            }

            var memberName = memberAccess.Name.ToString();
            if (!methodNames.Contains(memberName))
            {
                return false;
            }


            var searchedTargetType = context.SemanticModel.Compilation.GetTypeByMetadataName(accessedType);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (searchedTargetType == null ||
                actualTargetType == null ||
                !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return false;
            }

            return true;

        }

        // To be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPostConditions(SyntaxNodeAnalysisContext expression, InvocationExpressionSyntax invocationExpression)
        {
            return true;
        }

        protected virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, InvocationExpressionSyntax invocationExpression)
        {
            var diagnosticFormatter = GetDiagnosticFormatter();
            var usedAs = diagnosticFormatter.GetDiagnosedUsage(invocationExpression);
            var location = diagnosticFormatter.GetLocation(invocationExpression);

            return Diagnostic.Create(rule, location, usedAs);
        }
    }
}