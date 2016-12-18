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
        private readonly IDiagnosticFormatter _diagnosticFormatter;

        protected BaseMemberInvocationAnalyzer()
        {
            _diagnosticFormatter = DiagnosticFormatterFactory.CreateMemberInvocationFormatter();
        }

        protected BaseMemberInvocationAnalyzer(IDiagnosticFormatter diagnosticFormatter)
        {
            _diagnosticFormatter = diagnosticFormatter;
        }

        protected void RegisterAction(DiagnosticDescriptor rule, AnalysisContext context, string accessedType, string memberName, params string[] additionalMemberNames)
        {
            // TODO create and initialize setnew HashSet<string>();
            var forbiddenMemberNames = new[] {memberName}.Concat(additionalMemberNames);

            context.RegisterSyntaxNodeAction(c => Analyze(rule, c, accessedType, forbiddenMemberNames), SyntaxKind.InvocationExpression);
        }

        // Change to ISEt
        private void Analyze(DiagnosticDescriptor rule, SyntaxNodeAnalysisContext context, string accessedType, IEnumerable<string> forbiddenMemberNames)
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

            var searchedTargetType = TypeExtensions.GetITypeSymbol(accessedType, context);
            var actualTargetType = new SemanticModelBrowser(context).GetMemberAccessTarget(memberAccess) as INamedTypeSymbol;
            if (searchedTargetType == null || actualTargetType == null || !actualTargetType.IsDerivedFromClassOrInterface(searchedTargetType))
            {
                return;
            }

            var usedAs = _diagnosticFormatter.GetDiagnosedUsage(invocationExpression);
            var location = _diagnosticFormatter.GetLocation(invocationExpression);
            var diagnostic = Diagnostic.Create(rule, location, usedAs);

            context.ReportDiagnostic(diagnostic);
        }
    }
}