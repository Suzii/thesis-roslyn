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
    public abstract class BaseMemberInvocationAnalyzer : BaseMemberAccessOrInvocationAnalyzer<InvocationExpressionSyntax>
    {
        private static readonly IDiagnosticFormatter _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationFormatter();

        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string methodName, params string[] additionalMethodNames)
        {
            var forbiddenMemberNames = new HashSet<string>(additionalMethodNames) { methodName };

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.InvocationExpression);
        }

        protected override IDiagnosticFormatter DiagnosticFormatter => _diagnosticFormatter;

        protected override bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, ISet<string> methodNames)
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
            var actualTargetType = context.SemanticModel.GetTypeInfo(memberAccess.Expression).Type as INamedTypeSymbol;
            if (searchedTargetType == null || actualTargetType == null)
            {
                return false;
            }

            return actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType);
        }
    }
}