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
    public class MemberInvocationAnalyzer : ISyntaxNodeAnalyzer
    {
        protected readonly ApiReplacementConfig Config;
        protected readonly IDiagnosticFormatter<InvocationExpressionSyntax> Formatter;
        
        public MemberInvocationAnalyzer(ApiReplacementConfig config, IDiagnosticFormatter<InvocationExpressionSyntax> formatter)
        {
            Config = config;
            Formatter = formatter;
        }

        public void Run(SyntaxNodeAnalysisContext context)
        {
            var filePath = context.Node?.SyntaxTree?.FilePath;
            if (!IsOnForbiddenPath(filePath))
            {
                return;
            }

            var invocation = (InvocationExpressionSyntax)context.Node;
            if (invocation == null)
            {
                return;
            }

            IMethodSymbol methodSymbol;
            if (!IsForbiddenMethodOnForbiddenType(context, invocation, out methodSymbol))
            {
                return;
            }

            if (!IsForbiddenUsage(methodSymbol))
            {
                return;
            }

            var diagnostic = CreateDiagnostic(invocation);
            context.ReportDiagnostic(diagnostic);
        }

        protected virtual bool IsOnForbiddenPath(string filePath) => true;

        protected bool IsForbiddenMethodOnForbiddenType(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, out IMethodSymbol methodSymbol)
        {
            if (CanBeSkippedBasedOnSyntaxOnly(invocation))
            {
                methodSymbol = null;
                return false;
            }
            
            var invokedMethodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            methodSymbol = invokedMethodSymbol;
            if (invokedMethodSymbol == null)
            {
                return false;
            }

            if (!Config.ForbiddenMembers.Contains(invokedMethodSymbol.Name))
            {
                return false;
            }

            var receiverTypeSymbol = invokedMethodSymbol.ReceiverType as INamedTypeSymbol;
            if (receiverTypeSymbol == null || 
                Config.ForbiddenTypes.All(forbiddenType => !receiverTypeSymbol.IsDerivedFrom(forbiddenType, context.Compilation)))
            {
                return false;
            }

            return true;
        }

        private bool CanBeSkippedBasedOnSyntaxOnly(InvocationExpressionSyntax invocationExpression)
        {
            // either has underlying member access expression
            if (invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var memberAccess = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                // invoked member is not one of forbidden member names, can skip
                return !Config.ForbiddenMembers.Contains(memberAccess?.Name?.Identifier.ValueText);
            }
            // or was invoked within conditional access expression

            if (invocationExpression.Expression.IsKind(SyntaxKind.MemberBindingExpression))
            {
                var memberBinding = (MemberBindingExpressionSyntax) invocationExpression.Expression;
                return !Config.ForbiddenMembers.Contains(memberBinding?.Name?.Identifier.ValueText);
            }

            // or is not a member invocation but a direct one
            return true;
        }

        protected virtual bool IsForbiddenUsage(IMethodSymbol methodSymbol) => true;

        protected virtual Diagnostic CreateDiagnostic(InvocationExpressionSyntax invocation)
        {
            var location = Formatter.GetLocation(invocation);
            var diagnosedUsage = Formatter.GetDiagnosedUsage(invocation);
            var diagnostic = Diagnostic.Create(Config.Rule, location, diagnosedUsage);

            return diagnostic;
        }
    }
}