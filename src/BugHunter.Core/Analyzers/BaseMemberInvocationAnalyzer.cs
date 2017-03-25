using System.Collections.Generic;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
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
        private static readonly IDiagnosticFormatter<InvocationExpressionSyntax> _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationFormatter();

        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string methodName, params string[] additionalMethodNames)
        {
            var forbiddenMemberNames = new HashSet<string>(additionalMethodNames) { methodName };

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.InvocationExpression);
        }

        protected virtual IDiagnosticFormatter<InvocationExpressionSyntax> DiagnosticFormatter => _diagnosticFormatter;

        protected void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, ISet<string> forbiddenMemberNames)
        {
            if (!IsOnForbiddenPath(context.Node?.SyntaxTree?.FilePath))
            {
                return;
            }

            if (!CheckMainConditions(context, accessedType, forbiddenMemberNames))
            {
                return;
            }

            var syntaxNode = context.Node as InvocationExpressionSyntax;
            var invokedMethodSymbol = context.SemanticModel.GetSymbolInfo(syntaxNode).Symbol as IMethodSymbol;
            if (!CheckPostConditions(context, syntaxNode, invokedMethodSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(rule, syntaxNode);
            context.ReportDiagnostic(diagnostic);
        }

        // Can be overriden by subClasses if additional checks are needed
        protected virtual bool IsOnForbiddenPath(string filePath) => true;

        protected bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, ISet<string> methodNames)
        {
            var invocationExpression = (InvocationExpressionSyntax)context.Node;
            var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;

            // invocation expression is not part of member access
            if (memberAccess == null)
            {
                // TODO it can be child of a ConditionalAccessExpression
                return false;
            }

            var memberName = memberAccess.Name.ToString();
            if (!methodNames.Contains(memberName))
            {
                return false;
            }

            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;

            return actualTargetType?.IsDerivedFrom(accessedType, context.Compilation) ?? false;
        }

        // To be overriden by subClasses if additional checks are needed
        protected virtual bool CheckPostConditions(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax syntaxNode, IMethodSymbol methodSymbol) => true;

        protected virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, InvocationExpressionSyntax syntaxNode)
        {
            var usedAs = DiagnosticFormatter.GetDiagnosedUsage(syntaxNode);
            var location = DiagnosticFormatter.GetLocation(syntaxNode);

            return Diagnostic.Create(rule, location, usedAs);
        }
    }
}