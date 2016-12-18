using System.Collections.Generic;
using System.Linq;
using BugHunter.Core.DiagnosticsFormatting;
using BugHunter.Core.Extensions;
using BugHunter.Core.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BugHunter.CsRules.Analyzers
{
    /// <summary>
    /// Used for analysis of MemberAccess which Invocation at the same time such as "whereCondition.WhereLike(...)"
    /// </summary>
    public abstract class BaseMemberInvocationAnalyzer : DiagnosticAnalyzer
    {
        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string memberName, params string[] additionalMemberNames)
        {
            // TODO create and initialize setnew HashSet<string>();
            var forbiddenMemberNames = new[] { memberName }.Concat(additionalMemberNames);

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.InvocationExpression);
        }

        protected virtual IDiagnosticFormatter GetDiagnosticFormatter()
        {
            return DiagnosticFormatterFactory.CreateMemberInvocationFormatter();
        }

        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, IEnumerable<string> forbiddenMemberNames)
        {
            if (!CheckPreConditions(context))
            {
                return;
            }

            if (!CheckMainConditions(context, accessedType, forbiddenMemberNames))
            {
                return;
            }

            var invocationExpression = context.Node as InvocationExpressionSyntax;
            if (!CheckPostConditions(invocationExpression))
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

        protected virtual bool CheckMainConditions(SyntaxNodeAnalysisContext context, string accessedType, IEnumerable<string> methodNames)
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

            var searchedTargetType = TypeExtensions.GetITypeSymbol(accessedType, context);
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
        protected virtual bool CheckPostConditions(InvocationExpressionSyntax memberAccess)
        {
            return true;
        }

        protected virtual Diagnostic CreateDiagnostic(DiagnosticDescriptor rule, InvocationExpressionSyntax memberAccess)
        {
            var diagnosticFormatter = GetDiagnosticFormatter();
            var usedAs = diagnosticFormatter.GetDiagnosedUsage(memberAccess);
            var location = diagnosticFormatter.GetLocation(memberAccess);

            return Diagnostic.Create(rule, location, usedAs);
        }
    }
}